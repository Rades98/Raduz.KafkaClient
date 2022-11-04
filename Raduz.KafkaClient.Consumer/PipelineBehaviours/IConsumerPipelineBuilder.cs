using Avro.Specific;

namespace Raduz.KafkaClient.Consumer
{
	internal interface IConsumerPipelineBuilder
	{
		void Execute(HandlerDelegate input);

		event Action<Task> Finished;

		public IConsumerPipelineBuilder Build();

		void AddStep(IConsumerPipelineBehaviour behaviour, ISpecificRecord data, string topicName);

		void AddSteps(IEnumerable<IConsumerPipelineBehaviour> pipelineBehaviours, ISpecificRecord data, string topicName);
	}
}
