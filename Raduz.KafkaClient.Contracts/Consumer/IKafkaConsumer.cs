using Avro.Specific;
using Raduz.KafkaClient.Contracts.Requests;

namespace Raduz.KafkaClient.Consumer
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
