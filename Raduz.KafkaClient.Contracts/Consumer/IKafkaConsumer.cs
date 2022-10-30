using Avro.Specific;
using Raduz.KafkaClient.Contracts.Consumer.Handler;

namespace Raduz.KafkaClient.Contracts.Consumer
{
	/// <summary>
	/// Kafka consumer interface
	/// </summary>
	public interface IKafkaConsumer
	{
		string TopicName { get; }

		Type Schema { get; }

		IKafkaConsumerHandler<ISpecificRecord> Handler { get; }
	}
}
