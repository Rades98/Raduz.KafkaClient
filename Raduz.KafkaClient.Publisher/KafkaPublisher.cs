using Avro.Specific;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using Raduz.KafkaClient.Contracts.Configuration;
using Raduz.KafkaClient.Contracts.Publisher;

namespace Raduz.KafkaClient.Publisher
{
	public sealed class KafkaPublisher : IKafkaPublisher
	{
		private readonly KafkaClientProducerConfig _producerConfig;
		private readonly SchemaRegistryConfig _schemaRegistryConfig;
		private readonly IPublisherExceptionHandler? _exceptionHandler;

		public KafkaPublisher(
			IOptions<KafkaClientProducerConfig> producerConfig,
			IOptions<SchemaRegistryConfig> schemaRegistryConfig,
			IPublisherExceptionHandler exceptionHandler)
		{
			_producerConfig = producerConfig?.Value ?? throw new ArgumentNullException(nameof(producerConfig));
			_schemaRegistryConfig = schemaRegistryConfig.Value ?? throw new ArgumentNullException(nameof(schemaRegistryConfig));
			_exceptionHandler = exceptionHandler;
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
				if (_producerConfig.AllowCreateTopic)
				{
					await CreateTopicIfNotExists(topicName);
				}

				await producer.ProduceAsync(topicName, new Message<string, T> { Key = key, Value = data }, ct)
					.ContinueWith(task =>
					{
						if (task.IsFaulted)
						{
							_exceptionHandler?.Handle(task.Exception);
						}
					}, ct);
			}
			catch (Exception ex)
			{
				_exceptionHandler?.Handle(ex);
			}
		}

		private async Task CreateTopicIfNotExists(string topicName)
		{
			using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _producerConfig.BootstrapServers }).Build();
			bool topicExists = adminClient.GetMetadata(TimeSpan.FromSeconds(20)).Topics.Any(x => x.Topic == topicName);

			if (!topicExists)
			{
				try
				{
					await adminClient.CreateTopicsAsync(new TopicSpecification[]
					{
						new TopicSpecification { Name = topicName, ReplicationFactor = 2, NumPartitions = 5 }
					});
				}
				catch (CreateTopicsException e)
				{
					_exceptionHandler?.Handle(e);
				}
			}
		}
	}
}
