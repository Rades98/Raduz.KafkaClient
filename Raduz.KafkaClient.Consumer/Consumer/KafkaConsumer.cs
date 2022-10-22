using Avro.Specific;
using Raduz.KafkaClient.Contracts.Consumer;
using Raduz.KafkaClient.Contracts.Requests;

namespace Raduz.KafkaClient.Consumer.Consumer
{
	/// <summary>
	/// KAfka consumer
	/// </summary>
	/// <typeparam name="TRecord">Specific AVRO record</typeparam>
	/// <typeparam name="TRequest">Kafka client request <seealso cref="KafkaClientRequest{T}"/></typeparam>
	public sealed class KafkaConsumer<TRecord, TRequest> : IKafkaConsumer where TRecord : ISpecificRecord where TRequest : KafkaClientRequest<TRecord>
	{
		/// <summary>
		/// Name of consumed topic
		/// </summary>
		public string TopicName { get; }

		/// <summary>
		/// Type of AVRO
		/// </summary>
		public Type Schema { get; }

		public KafkaConsumer(string topicName)
		{
			TopicName = topicName;
			Schema = typeof(TRecord);
		}

		/// <summary>
		/// Get mediator request
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public IKafkaClientRequest GetRequest(ISpecificRecord data) => (IKafkaClientRequest)Activator.CreateInstance(typeof(TRequest), data)!;
	}
}
