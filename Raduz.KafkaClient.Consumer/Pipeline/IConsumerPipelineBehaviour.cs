namespace Raduz.KafkaClient.Consumer.Pipeline
{
	public interface IConsumerPipelineBehaviour
	{
		Task<bool> Handle(HandlerDelegate next, CancellationToken ct);
	}
}
