using Avro.Specific;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Raduz.KafkaClient.Contracts.Configuration;
using Raduz.KafkaClient.Contracts.Publisher;

namespace Raduz.KafkaClient.Publisher
{
	public sealed class KafkaPublisher : IKafkaPublisher
	{
		private readonly KafkaClientProducerConfig _producerConfig;
		private readonly SchemaRegistryConfig _schemaRegistryConfig;
		private readonly IPublisherExceptionHandler? _exceptionHandler;
		private readonly IMemoryCache _memoryCache;

		static readonly object _lock = new object();
		private static readonly TimeSpan defaultExpiration = TimeSpan.FromSeconds(60);

		public KafkaPublisher(
			IOptions<KafkaClientProducerConfig> producerConfig,
			IOptions<SchemaRegistryConfig> schemaRegistryConfig,
			IMemoryCache memoryCache,
			IPublisherExceptionHandler? exceptionHandler = null)
		{
			_producerConfig = producerConfig?.Value ?? throw new ArgumentNullException(nameof(producerConfig));
			_schemaRegistryConfig = schemaRegistryConfig.Value ?? throw new ArgumentNullException(nameof(schemaRegistryConfig));
			_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
			_exceptionHandler = exceptionHandler;
		}

		/// <inheritdoc/>
		public async Task PublishAsync<T>(string topicName, string key, T data, CancellationToken ct)
			where T : class, ISpecificRecord
		{
			var producer = GetProducer<T>();
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
							if (_exceptionHandler is not null)
							{
								_exceptionHandler.Handle(task.Exception);
							}
							else
							{
								throw task.Exception ?? new Exception("Some error has occured");
							}
						}
					}, ct);
			}
			catch (Exception ex)
			{
				if (_exceptionHandler is not null)
				{
					await _exceptionHandler.Handle(ex);
				}
				else
				{
					throw;
				}
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

		private IProducer<string, T> GetProducer<T>()
		{
			var producer = _memoryCache.Get(nameof(T)) as IProducer<string, T>;

			if (producer is not null)
			{
				return producer;
			}

			lock (_lock)
			{
				if (producer is not null)
				{
					return producer;
				}

				var tempProducer = CreateNewProducer<T>();

				_memoryCache.Set(nameof(T), tempProducer, GetOptions<T>(tempProducer, _producerConfig.MessageTimeoutMs ?? 1000));

				producer = tempProducer;
			}

			return producer;
		}

		private IProducer<string, T> CreateNewProducer<T>()
			 => new ProducerBuilder<string, T>(_producerConfig)
					.SetValueSerializer(new AvroSerializer<T>(new CachedSchemaRegistryClient(_schemaRegistryConfig), new AvroSerializerConfig()
					{
						BufferBytes = 100,
					}))
					.Build();

		internal static MemoryCacheEntryOptions GetOptions<T>(IProducer<string, T> producer, int messageTimeoutMs)
		{
			var tokenRegistration = new CancellationTokenSource();
			tokenRegistration.CancelAfter(defaultExpiration);

			var producerFlushToken = new CancellationTokenSource();

			var cachePreremovalToken = new CancellationChangeToken(tokenRegistration.Token);
			cachePreremovalToken.RegisterChangeCallback(obj =>
			{
				if (obj is IProducer<string, T> producer)
				{
					producer.Flush();
					producerFlushToken.CancelAfter(messageTimeoutMs);
				}
			}, producer);

			var opt = new MemoryCacheEntryOptions();
			opt.AddExpirationToken(cachePreremovalToken);

			var cacheExpirationToken = new CancellationChangeToken(producerFlushToken.Token);
			cacheExpirationToken.RegisterChangeCallback(obj =>
			{
				if (obj is IProducer<string, T> producer)
				{
					producer.Dispose();
				}
			}, producer);

			return opt;
		}
	}
}
