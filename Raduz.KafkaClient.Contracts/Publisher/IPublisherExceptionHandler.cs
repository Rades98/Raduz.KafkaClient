namespace Raduz.KafkaClient.Contracts.Publisher
{
	public interface IPublisherExceptionHandler
	{
		Task Handle(Exception? e);
	}
}
