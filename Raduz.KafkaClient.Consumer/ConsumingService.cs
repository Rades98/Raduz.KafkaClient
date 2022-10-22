using Avro.Specific;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raduz.KafkaClient.Common.Extensions;
using Raduz.KafkaClient.Consumer.Consumer;
using YCherkes.SchemaRegistry.Serdes.Avro;

namespace Raduz.KafkaClient.Consumer
{
	internal class ConsumingService : BackgroundService
	{
		private readonly ILogger<ConsumingService> _logger;
		private readonly ConsumerConfig _consumerConfig;
		private readonly SchemaRegistryConfig _schemaRegistryConfig;
		private readonly IMediator _mediator;
		private readonly KafkaConsumerCollections _kafkaClientOptions;

		private readonly CancellationTokenSource _cancellationTokenSource;

		public ConsumingService(
			ILogger<ConsumingService> logger,
			IOptions<ConsumerConfig> consumerConfig,
			IOptions<SchemaRegistryConfig> schemaRegistryConfig,
			IMediator mediator,
			KafkaConsumerCollections kafkaClientOptions)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_consumerConfig = consumerConfig.Value ?? throw new ArgumentNullException(nameof(consumerConfig));
			_schemaRegistryConfig = schemaRegistryConfig.Value ?? throw new ArgumentNullException(nameof(schemaRegistryConfig));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_kafkaClientOptions = kafkaClientOptions ?? throw new ArgumentNullException(nameof(kafkaClientOptions));

			_cancellationTokenSource = new CancellationTokenSource();
		}

		public override Task StartAsync(CancellationToken cancellationToken)
		{
			cancellationToken.Register(() => _cancellationTokenSource.Cancel());
			return base.StartAsync(cancellationToken);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig);
			using var consumer = new ConsumerBuilder<string, ISpecificRecord>(_consumerConfig)
				.SetLogHandler((_, message) => _logger.Log(message.Level.ToLogLevel(), "Kafka _logger: {name} {message}", message.Name, message.Message))
				.SetValueDeserializer(new MultiSchemaAvroDeserializer(schemaRegistry).AsSyncOverAsync())
				.Build();

			try
			{
				consumer.Subscribe(_kafkaClientOptions.ConsumerRegistrations.Values.Select(topic => topic.TopicName));
				_logger.LogInformation("Consumption of {topics}", _kafkaClientOptions.ConsumerRegistrations.Select(topic => $"\n{topic.Value.TopicName}"));

				var cancelToken = new CancellationTokenSource();

				while (true)
				{
					var result = consumer.Consume(cancelToken.Token);
					string schemaName = result.Message.Value.Schema.Name;

					if (_kafkaClientOptions.ConsumerRegistrations.Any(topic => topic.Key == schemaName))
					{
						var data = result.Message.Value;

						_logger.LogInformation("Obtained data of type: {type}", schemaName);

						if (await _mediator.Send(_kafkaClientOptions.ConsumerRegistrations[schemaName].GetRequest(data), cancelToken.Token))
						{
							_logger.LogInformation("Topic of type {type} handled", schemaName);
						}
						else
						{
							_logger.LogError("Failed while handling topic of type {type}", schemaName);
						}
					}
				}
			}
			catch (Exception e)
			{
				_logger.LogError("{error}", e);
			}
			finally
			{
				consumer.Close();
			}
		}
	}
}
