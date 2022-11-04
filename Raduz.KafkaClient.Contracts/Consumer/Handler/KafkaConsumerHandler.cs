using Avro.Specific;
using Raduz.KafkaClient.Consumer;

namespace Raduz.KafkaClient.Contracts.Consumer.Handler
{
	public abstract class KafkaConsumerHandler<TRecord> : IKafkaConsumerHandler<TRecord> where TRecord : ISpecificRecord
	{

		protected readonly IConsumerManager? _consumerManager;

		/// <inheritdoc/>
		public string TopicName { get; }

		/// <inheritdoc/>
		public string Schema { get; }

		/// <inheritdoc/>
		public abstract Task HandleAsync(TRecord record, CancellationToken ct);

		/// <inheritdoc/>
		public Task HandleRecordAsync(ISpecificRecord record, CancellationToken ct) => HandleAsync((TRecord)record, ct);

		public KafkaConsumerHandler(string topicName, IConsumerManager consumerManager)
		{
			TopicName = topicName;
			Schema = typeof(TRecord).Name;

			_consumerManager = consumerManager ?? throw new ArgumentNullException(nameof(consumerManager));
		}

		public KafkaConsumerHandler(string topicName)
		{
			TopicName = topicName;
			Schema = typeof(TRecord).Name;
		}

		public void PauseConsumption()
		{
			if(_consumerManager is not null)
			{
				_consumerManager.Pause();
			}
			else
			{
				throw new NotImplementedException(nameof(_consumerManager));
			}
		}

		public void ReturnConsumption()
		{
			if (_consumerManager is not null)
			{
				_consumerManager.Return();
			}
			else
			{
				throw new NotImplementedException(nameof(_consumerManager));
			}
		}
	}
}
