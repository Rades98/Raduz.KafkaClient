using Avro.Specific;

namespace Raduz.KafkaClient.Consumer
{
	public interface IConsumerPipelineBehaviour
	{
		Task Handle(HandlerDelegate next, ISpecificRecord data, string topicName, CancellationToken ct);
	}
}
