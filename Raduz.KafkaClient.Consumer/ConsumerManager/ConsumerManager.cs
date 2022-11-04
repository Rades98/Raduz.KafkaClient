
namespace Raduz.KafkaClient.Consumer
{
	public sealed class ConsumerManager : IConsumerManager
	{
		public bool IsRunning { get; set; }

		public event EventHandler<IConsumerManagerEventArgs> RunStateChange = delegate { };

		public void Pause()
		{
			RunStateChange(this, new ConsumerManagerEventArgs() { Run = false });
		}

		public void Return()
		{
			RunStateChange(this, new ConsumerManagerEventArgs() { Run = true });
		}
	}
}
