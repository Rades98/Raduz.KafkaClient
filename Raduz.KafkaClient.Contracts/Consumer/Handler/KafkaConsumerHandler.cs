using Avro.Specific;

namespace Raduz.KafkaClient.Consumer
{
	public abstract class KafkaConsumerHandler<TRecord> : IKafkaConsumerHandler<TRecord> where TRecord : ISpecificRecord
	{
		/// <inheritdoc/>
		public string TopicName { get; }

		/// <inheritdoc/>
		public string Schema { get; }

		/// <inheritdoc/>
		public abstract Task<bool> HandleAsync(TRecord record, CancellationToken ct);

		/// <inheritdoc/>
		public Task<bool> HandleRecordAsync(ISpecificRecord record, CancellationToken ct) => HandleAsync((TRecord)record, ct);

		public KafkaConsumerHandler(string topicName)
		{
			TopicName = topicName;
			Schema = typeof(TRecord).Name;
		}
	}
}
