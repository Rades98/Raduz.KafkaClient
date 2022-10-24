using Avro.Specific;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raduz.KafkaClient.Common.Extensions;
using YCherkes.SchemaRegistry.Serdes.Avro;

namespace Raduz.KafkaClient.Consumer
{
	/// <summary>
	/// Hosted service consuming kafka topics provided 
	/// by registration and calling its handler 
	/// by its request via MediatR
	/// </summary>
	internal sealed class ConsumingService : BackgroundService
	{
		private readonly ILogger<ConsumingService> _logger;
		private readonly ConsumerConfig _consumerConfig;
		private readonly SchemaRegistryConfig _schemaRegistryConfig;
		private readonly IEnumerable<IKafkaHandler> _handlers;

		private readonly CancellationTokenSource _cancellationTokenSource;
		private const int MAX_CONSUME_RETRY_COUNT = 1;

		public ConsumingService(
			ILogger<ConsumingService> logger,
			IOptions<ConsumerConfig> consumerConfig,
			IOptions<SchemaRegistryConfig> schemaRegistryConfig,
			IEnumerable<IKafkaHandler> handlers)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_consumerConfig = consumerConfig.Value ?? throw new ArgumentNullException(nameof(consumerConfig));
			_schemaRegistryConfig = schemaRegistryConfig.Value ?? throw new ArgumentNullException(nameof(schemaRegistryConfig));
			_handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));

			_cancellationTokenSource = new CancellationTokenSource();
		}

		public override Task StartAsync(CancellationToken cancellationToken)
		{
			cancellationToken.Register(() => _cancellationTokenSource.Cancel());
			return base.StartAsync(cancellationToken);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			int retries = 0;
			ConsumeResult<string, ISpecificRecord> consumeResult = new();

			using var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig);
			using var consumer = new ConsumerBuilder<string, ISpecificRecord>(_consumerConfig)
				.SetLogHandler((_, message) => _logger.Log(message.Level.ToLogLevel(), "Kafka _logger: {name} {message}", message.Name, message.Message))
				.SetValueDeserializer(new MultiSchemaAvroDeserializer(schemaRegistry).AsSyncOverAsync())
				.Build();

			try
			{
				consumer.Subscribe(_handlers.Select(handler => handler.TopicName));
				_logger.LogInformation("Consumption of {topics}", _handlers.Select(handler => $"\n{handler.TopicName}"));

				var cancellationToken = new CancellationTokenSource().Token;

				while (true)
				{
					try
					{
						consumeResult = consumer.Consume(cancellationToken);
						string schemaName = consumeResult.Message.Value.Schema.Name;

						if (_handlers.Any(handler => handler.Schema == schemaName))
						{
							var data = consumeResult.Message.Value;

							_logger.LogInformation("Obtained data of type: {type}", schemaName);

							if (await _handlers.First(handler => handler.Schema == schemaName).HandleRecordAsync(data, cancellationToken))
							{
								_logger.LogInformation("Topic of type {type} handled", schemaName);
							}
							else
							{
								_logger.LogError("Failed while handling topic of type {type}", schemaName);

							}
						}

						retries = 0;
					}
					catch (Exception e)
					{
						_logger.LogError("KAfka consumption failed with error: {error}", e);

						if (retries < MAX_CONSUME_RETRY_COUNT)
						{
							_logger.LogInformation("Trying to consume again (refreshing partition offset) attempt {attempt} of {maxAttmepts}", retries, MAX_CONSUME_RETRY_COUNT);

							//retry by resetting consumer offset
							consumer.Assign(consumeResult.TopicPartitionOffset);

							retries++;
						}
						else
						{
							retries = 0;
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
