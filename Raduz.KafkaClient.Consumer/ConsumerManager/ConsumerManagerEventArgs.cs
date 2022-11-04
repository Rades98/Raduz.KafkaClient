namespace Raduz.KafkaClient.Consumer
{
	public class ConsumerManagerEventArgs : EventArgs, IConsumerManagerEventArgs
	{
		public bool Run { get; set; }
	}
}
