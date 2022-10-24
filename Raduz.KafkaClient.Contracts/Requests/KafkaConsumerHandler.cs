using Avro.Specific;

namespace Raduz.KafkaClient.Contracts.Requests
{
	public abstract class KafkaConsumerHandler<TRecord> : IKafkaConsumerHandler<TRecord> where TRecord : ISpecificRecord
	{
		public string TopicName { get; }

		public string Schema { get; }

		public abstract Task<bool> HandleAsync(TRecord record, CancellationToken ct);
		public Task<bool> HandleRecordAsync(ISpecificRecord record, CancellationToken ct) => HandleAsync((TRecord)record, ct);

		public KafkaConsumerHandler(string topicName)
		{
			TopicName = topicName;
			Schema = typeof(TRecord).Name;
		}
	}
}
