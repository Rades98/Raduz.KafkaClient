using Avro.Specific;

namespace Raduz.KafkaClient.Contracts.Requests
{
	public interface IKafkaHandler
	{
		public string TopicName { get; }

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
