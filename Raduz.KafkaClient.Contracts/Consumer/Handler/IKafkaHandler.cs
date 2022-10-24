using Avro.Specific;

namespace Raduz.KafkaClient.Consumer
{
	/// <summary>
	/// Kafka handler base type
	/// </summary>
	public interface IKafkaHandler
	{
		/// <summary>
		/// Name of consummed topic
		/// </summary>
		public string TopicName { get; }

		/// <summary>
		/// AVRO schema name
		/// </summary>
		public string Schema { get; }

		/// <summary>
		/// Handle obtained record
		/// </summary>
		/// <param name="record">Obtained data from cunsumption of the topic</param>
		/// <param name="ct">Cancellation token</param>
		/// <returns></returns>
		public Task<bool> HandleRecordAsync(ISpecificRecord record, CancellationToken ct);
	}
}
