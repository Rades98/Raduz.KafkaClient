using Avro.Specific;

namespace Raduz.KafkaClient.Consumer
{
	internal interface IConsumerPipelineBuilder
	{
		void Execute(HandlerDelegate input);

		event Action<bool> Finished;

		public IConsumerPipelineBuilder Build();

		void AddStep(IConsumerPipelineBehaviour behaviour, ISpecificRecord data);

		void AddSteps(IEnumerable<IConsumerPipelineBehaviour> pipelineBehaviours, ISpecificRecord data);
	}
}
