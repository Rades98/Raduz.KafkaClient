using Avro.Specific;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raduz.KafkaClient.Contracts.Publisher;

namespace Raduz.KafkaClient.Publisher
{
	public sealed class KafkaPublisher : IKafkaPublisher
	{
		private readonly ProducerConfig _producerConfig;
		private readonly ILogger<KafkaPublisher> _logger;
		private readonly SchemaRegistryConfig _schemaRegistryConfig;

		public KafkaPublisher(IOptions<ProducerConfig> producerConfig, ILogger<KafkaPublisher> logger, IOptions<SchemaRegistryConfig> schemaRegistryConfig)
		{
			_producerConfig = producerConfig?.Value ?? throw new ArgumentNullException(nameof(producerConfig));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_schemaRegistryConfig = schemaRegistryConfig.Value ?? throw new ArgumentNullException(nameof(schemaRegistryConfig));
		}

		/// <inheritdoc/>
		public async Task PublishAsync<T>(string topicName, string key, T data, CancellationToken ct)
			where T : class, ISpecificRecord
		{
			using var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig);
			using var producer = new ProducerBuilder<string, T>(_producerConfig)
				.SetValueSerializer(new AvroSerializer<T>(schemaRegistry, new AvroSerializerConfig()
				{
					BufferBytes = 100,
				}))
				.Build();
			try
			{
				_logger.LogInformation("Producing of type {type}", typeof(T));
				await producer.ProduceAsync(topicName, new Message<string, T> { Key = key, Value = data })
					.ContinueWith(task =>
					{
						if (!task.IsFaulted)
						{
							_logger.LogInformation("Producing {Key} to topic {topic} : {Data} published successfully.", key, topicName, data);
							return;
						}

						_logger.LogError("error producing message: {Exception} InnerException: {InnerException}", task.Exception, task.Exception?.InnerException);
					}, ct);
			}
			catch (Exception ex)
			{
				_logger.LogError("error producing message: {Exception}", ex);
			}

		}
	}
}
