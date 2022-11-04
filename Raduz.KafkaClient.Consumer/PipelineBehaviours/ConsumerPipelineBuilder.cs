using System.Collections.Concurrent;
using Avro.Specific;

namespace Raduz.KafkaClient.Consumer
{
	internal class ConsumerPipelineBuilder : IConsumerPipelineBuilder
	{
		List<Func<HandlerDelegate, Task>> _pipelineSteps = new();
		BlockingCollection<HandlerDelegate>[]? _buffers;
		CancellationTokenSource _ctSource;

		public event Action<Task> Finished = delegate { };

		public ConsumerPipelineBuilder()
		{
			_ctSource = new CancellationTokenSource();
		}

		public void AddStep(IConsumerPipelineBehaviour behaviour, ISpecificRecord data, string topicName)
		{
			_pipelineSteps.Add(input => behaviour.Handle(input, data, topicName, _ctSource.Token));
		}

		public void AddSteps(IEnumerable<IConsumerPipelineBehaviour> pipelineBehaviours, ISpecificRecord data, string topicName)
		{
			pipelineBehaviours.ToList().ForEach(behaviour => _pipelineSteps.Add(input => behaviour.Handle(input, data, topicName, _ctSource.Token)));
		}

		public void Execute(HandlerDelegate input)
		{
			var first = _buffers![0];
			first.Add(input);
		}

		public IConsumerPipelineBuilder Build()
		{
			_buffers = _pipelineSteps
				.Select(step => new BlockingCollection<HandlerDelegate>())
				.ToArray();

			int bufferIndex = 0;
			_pipelineSteps.ForEach(async pipelineStep =>
			{
				{
					int bufferIndexLocal = bufferIndex;
					await Task.Run(async () =>
					{
						foreach (var input in _buffers[bufferIndexLocal].GetConsumingEnumerable())
						{
							var output = pipelineStep.Invoke(input);

							bool isLastStep = bufferIndexLocal == _pipelineSteps.Count - 1;
							if (isLastStep)
							{
								Finished?.Invoke(output);
							}
							else
							{
								var next = _buffers[bufferIndexLocal + 1];
								next.Add(() => output);
							}
						}
					});

					bufferIndex++;
				}
			});
			
			return this;
		}
	}
}
