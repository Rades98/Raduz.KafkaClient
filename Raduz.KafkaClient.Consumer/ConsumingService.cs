using Avro.Specific;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Raduz.KafkaClient.Contracts.Configuration;
using Raduz.KafkaClient.Contracts.Consumer;
using Raduz.KafkaClient.Contracts.Consumer.Handler;
using YCherkes.SchemaRegistry.Serdes.Avro;

namespace Raduz.KafkaClient.Consumer
{
	/// <summary>
	/// Hosted service consuming kafka topics provided 
	/// by registration and calling its handler 
	/// </summary>
	internal sealed class ConsumingService : BackgroundService
	{
		private readonly KafkaClientConsumerConfig _consumerConfig;
		private readonly SchemaRegistryConfig _schemaRegistryConfig;
		private readonly IEnumerable<IKafkaHandler> _handlers;
		private readonly IConsumerExceptionHandler? _exceptionHandler;

		private readonly CancellationTokenSource _cancellationTokenSource;

		public ConsumingService(
			IOptions<KafkaClientConsumerConfig> consumerConfig,
			IOptions<SchemaRegistryConfig> schemaRegistryConfig,
			IEnumerable<IKafkaHandler> handlers,
			IConsumerExceptionHandler exceptionHandler)
		{
			_consumerConfig = consumerConfig.Value ?? throw new ArgumentNullException(nameof(consumerConfig));
			_schemaRegistryConfig = schemaRegistryConfig.Value ?? throw new ArgumentNullException(nameof(schemaRegistryConfig));
			_handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
			_exceptionHandler = exceptionHandler;

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
			ConsumeResult<string, ISpecificRecord>? consumeResult = null;

			using var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig);
			using var consumer = new ConsumerBuilder<string, ISpecificRecord>(_consumerConfig)
				.SetValueDeserializer(new MultiSchemaAvroDeserializer(schemaRegistry).AsSyncOverAsync())
				.Build();

			try
			{
				consumer.Subscribe(_handlers.Select(handler => handler.TopicName));

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

							if (!(await _handlers.First(handler => handler.Schema == schemaName).HandleRecordAsync(data, cancellationToken)))
							{
								_exceptionHandler?.Handle(new Exception($"Failed while handling topic of type {schemaName}"));
							}
						}

						retries = 0;
					}
					catch (ConsumeException e)
					{
						if (consumeResult is null)
						{
							retries = 0;
						}
						else
						{
							if (retries < _consumerConfig.MaxConsumeRetryCount)
							{
								//retry by resetting consumer offset
								consumer.Assign(consumeResult!.TopicPartitionOffset);

								retries++;
							}
							else
							{
								retries = 0;
								_exceptionHandler?.Handle(e);
							}
						}

					}
					catch (OperationCanceledException oe)
					{
						_exceptionHandler?.Handle(oe);
						consumer.Close();
						break;
					}
				}
			}
			catch (Exception e)
			{
				_exceptionHandler?.Handle(e);
			}
			finally
			{
				consumer.Close();
			}
		}
	}
}
