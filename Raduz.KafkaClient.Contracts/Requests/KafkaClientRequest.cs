using Avro.Specific;

namespace Raduz.KafkaClient.Contracts.Requests
{
	/// <summary>
	/// Kafka client request
	/// </summary>
	/// <typeparam name="T">AVRO specific record <seealso cref="ISpecificRecord"/></typeparam>
	public abstract class KafkaClientRequest<T> : IKafkaClientRequest where T : ISpecificRecord
	{
		public KafkaClientRequest(T specificRecord)
		{
			RequestValue = specificRecord;
		}

		/// <summary>
		/// Value of kafka consumed message
		/// </summary>
		public T RequestValue { get; }
	}
}
