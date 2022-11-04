using Avro.Specific;

namespace Raduz.KafkaClient.Contracts.Consumer.Handler
{
	/// <summary>
	/// IKafkaClientRequestHandler
	/// </summary>
	/// <typeparam name="TSpecificRecord"><seealso cref="IKafkaHandler"/></typeparam>
	public interface IKafkaConsumerHandler<TSpecificRecord> : IKafkaHandler where TSpecificRecord : ISpecificRecord
	{
		/// <summary>
		/// Handle obtained record
		/// </summary>
		/// <param name="record">Obtained data from cunsumption of the topic</param>
		/// <param name="ct">Cancellation token</param>
		/// <returns></returns>
		public Task HandleAsync(TSpecificRecord record, CancellationToken ct);
	}
}
