namespace Raduz.KafkaClient.Consumer
{
	public interface IConsumerManager
	{
		event EventHandler<IConsumerManagerEventArgs> RunStateChange;

		public bool IsRunning { get; set; }

		public void Pause();

		public void Return();
	}
}
