namespace Raduz.KafkaClient.Consumer.Pipeline
{
	internal interface IConsumerPipelineBuilder
	{
		void Execute(HandlerDelegate input);

		event Action<bool> Finished;

		public IConsumerPipelineBuilder Build();
	}
}
