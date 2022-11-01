using Avro.Specific;

namespace Raduz.KafkaClient.Consumer
{
	public interface IConsumerPipelineBehaviour
	{
		Task<bool> Handle(HandlerDelegate next, ISpecificRecord data, CancellationToken ct);
	}
}
