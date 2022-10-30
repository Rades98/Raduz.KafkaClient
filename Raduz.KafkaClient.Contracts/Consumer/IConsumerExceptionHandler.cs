namespace Raduz.KafkaClient.Contracts.Consumer
{
	public interface IConsumerExceptionHandler
	{
		Task Handle(Exception? e);
	}
}
