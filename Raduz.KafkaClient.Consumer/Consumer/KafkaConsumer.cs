using Avro.Specific;
using Raduz.KafkaClient.Contracts.Consumer;
using Raduz.KafkaClient.Contracts.Requests;

namespace Raduz.KafkaClient.Consumer.Consumer
{
	public class KafkaConsumer<TRecord, TRequest> : IKafkaConsumer where TRecord : ISpecificRecord where TRequest : KafkaClientRequest<TRecord>
	{
		public string TopicName { get; }

		public Type Schema { get; }

		public KafkaConsumer(string topicName)
		{
			TopicName = topicName;
			Schema = typeof(TRecord);
		}

		public IKafkaClientRequest GetRequest(ISpecificRecord data) => (IKafkaClientRequest)Activator.CreateInstance(typeof(TRequest), data)!;
	}
}
