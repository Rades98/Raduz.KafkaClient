namespace Raduz.KafkaClient.Consumer
{
	public interface IConsumingService
	{
		Task DoWork(CancellationToken stoppingToken);
	}
}
