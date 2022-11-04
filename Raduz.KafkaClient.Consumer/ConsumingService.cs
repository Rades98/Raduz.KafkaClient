using Avro.Specific;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Options;
using Raduz.KafkaClient.Consumer.Exceptions;
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
	public sealed class ConsumingService : IConsumingService
	{
		private readonly KafkaClientConsumerConfig _consumerConfig;
		private readonly SchemaRegistryConfig _schemaRegistryConfig;
		private readonly IEnumerable<IKafkaHandler> _handlers;
		private readonly IConsumerExceptionHandler? _exceptionHandler;
		private readonly IEnumerable<IConsumerPipelineBehaviour>? _pipelineBehaviours;

		private CancellationTokenSource _consumerCancellationTokenSource;
		private bool _run = true;

		public ConsumingService(
			IOptions<KafkaClientConsumerConfig> consumerConfig,
			IOptions<SchemaRegistryConfig> schemaRegistryConfig,
			IEnumerable<IKafkaHandler> handlers,
			IConsumerManager consumerManager,
			IConsumerExceptionHandler? exceptionHandler = null,
			IEnumerable<IConsumerPipelineBehaviour>? pipelineBehaviours = null)
		{
			_consumerConfig = consumerConfig.Value ?? throw new ArgumentNullException(nameof(consumerConfig));
			_schemaRegistryConfig = schemaRegistryConfig.Value ?? throw new ArgumentNullException(nameof(schemaRegistryConfig));
			_handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));

			_exceptionHandler = exceptionHandler;

			_pipelineBehaviours = pipelineBehaviours;

			_consumerCancellationTokenSource = new CancellationTokenSource();

			if (consumerManager is not null)
			{
				consumerManager.RunStateChange += delegate (object? sender, IConsumerManagerEventArgs e)
				{
					_run = e.Run;

					if (e.Run)
					{
						_consumerCancellationTokenSource = new();
					}
					else
					{
						_consumerCancellationTokenSource.Cancel();
					}
				};
			}
		}

		public async Task DoWork(CancellationToken stoppingToken)
		{
			//separatni thread
			int retries = 0;
			ConsumeResult<string, ISpecificRecord>? consumeResult = null;

			using var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig);
			using var consumer = new ConsumerBuilder<string, ISpecificRecord>(_consumerConfig)
				.SetValueDeserializer(new MultiSchemaAvroDeserializer(schemaRegistry).AsSyncOverAsync())
				.Build();

			try
			{
				consumer.Subscribe(_handlers.GroupBy(handler => handler.TopicName).Select(grp => grp.First().TopicName));

				var cancellationToken = _consumerCancellationTokenSource.Token;

				while (true)
				{
					if (_run)
					{
						try
						{
							consumeResult = consumer.Consume(cancellationToken);
							string schemaName = consumeResult.Message.Value.Schema.Name;
							string topicName = consumeResult.Topic;

							if (_handlers.Any(handler => handler.Schema == schemaName && handler.TopicName == topicName))
							{
								var data = consumeResult.Message.Value;

								if (_pipelineBehaviours is not null)
								{
									var builder = new ConsumerPipelineBuilder();

									builder.AddSteps(_pipelineBehaviours, data, topicName);

									var pipeline = builder.Build();
									pipeline.Execute(() => _handlers.First(handler => handler.Schema == schemaName).HandleRecordAsync(data, cancellationToken));
									pipeline.Finished += delegate { };
								}
								else
								{
									await _handlers.First(handler => handler.Schema == schemaName).HandleRecordAsync(data, cancellationToken);
								}
							}
							else
							{
								var exception = new NotImplementedHandlerException($"There is no implemented handler for topic: {topicName} with schema: {schemaName}");
								if (_exceptionHandler is not null)
								{
									await _exceptionHandler.Handle(exception);
								}
								else
								{
									throw exception;
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
									if (_exceptionHandler is not null)
									{
										await _exceptionHandler.Handle(e);
									}
									else
									{
										throw;
									}
								}
							}

						}
						catch (OperationCanceledException oe)
						{
							if (_run)
							{
								if (_exceptionHandler is not null)
								{
									await _exceptionHandler.Handle(oe);
								}
								else
								{
									throw;
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				throw;
			}
			finally
			{
				consumer.Close();
			}
		}
	}
}
