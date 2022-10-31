using System.Collections.Concurrent;

namespace Raduz.KafkaClient.Consumer.Pipeline
{
	internal class ConsumerPipelineBuilder : IConsumerPipelineBuilder
	{
		List<Func<HandlerDelegate, Task<bool>>> _pipelineSteps = new();
		BlockingCollection<HandlerDelegate>[]? _buffers;
		CancellationTokenSource _ctSource;

		public event Action<bool> Finished = delegate { };

		public ConsumerPipelineBuilder(IEnumerable<IConsumerPipelineBehaviour> pipelineBehaviours)
		{
			_ctSource = new CancellationTokenSource();
			pipelineBehaviours.ToList().ForEach(behaviour => _pipelineSteps.Add(input => behaviour.Handle(input, _ctSource.Token)));
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
						foreach (HandlerDelegate input in _buffers[bufferIndexLocal].GetConsumingEnumerable())
						{
							Task<bool> output = (Task<bool>)pipelineStep.Invoke(input);

							bool isLastStep = bufferIndexLocal == _pipelineSteps.Count - 1;
							if (isLastStep)
							{
								Finished?.Invoke(await output);
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
