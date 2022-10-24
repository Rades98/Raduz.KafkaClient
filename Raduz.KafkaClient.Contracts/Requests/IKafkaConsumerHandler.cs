using Avro.Specific;

namespace Raduz.KafkaClient.Contracts.Requests
{
	/// <summary>
	/// IKafkaClientRequestHandler
	/// </summary>
	/// <typeparam name="TRequest"><seealso cref="IKafkaClientRequest"/></typeparam>
	public interface IKafkaConsumerHandler<TSpecificRecord> : IKafkaHandler where TSpecificRecord : ISpecificRecord
	{
		/// <summary>
		/// Handle obtained record
		/// </summary>
		/// <param name="record">Obtained data from cunsumption of the topic</param>
		/// <param name="ct">Cancellation token</param>
		/// <returns></returns>
		public Task<bool> HandleAsync(TSpecificRecord record, CancellationToken ct);
	}
}
