using Avro.Specific;
using Raduz.KafkaClient.Contracts.Consumer;
using Raduz.KafkaClient.Contracts.Requests;

namespace Raduz.KafkaClient.Consumer.Consumer
{
	/// <summary>
	/// Kafka consumer
	/// </summary>
	/// <typeparam name="TRecord">Specific AVRO record</typeparam>
	/// <typeparam name="TRequest">Kafka client request <seealso cref="KafkaClientRequest{T}"/></typeparam>
	public sealed class KafkaConsumer<TRecord, TRequest> : IKafkaConsumer where TRecord : ISpecificRecord where TRequest : KafkaClientRequest<TRecord>
	{
		/// <inheritdoc/>
		public string TopicName { get; }

		/// <inheritdoc/>
		public Type Schema { get; }

		public KafkaConsumer(string topicName)
		{
			TopicName = topicName;
			Schema = typeof(TRecord);
		}

		/// <inheritdoc/>
		public IKafkaClientRequest GetRequest(ISpecificRecord data) => (IKafkaClientRequest)Activator.CreateInstance(typeof(TRequest), data)!;
	}
}
