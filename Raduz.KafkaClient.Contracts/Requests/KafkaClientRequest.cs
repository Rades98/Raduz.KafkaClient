using Avro.Specific;
using MediatR;

namespace Raduz.KafkaClient.Contracts.Requests
{
	public abstract class KafkaClientRequest<T> : IKafkaClientRequest where T : ISpecificRecord
	{
		public KafkaClientRequest(T specificRecord)
		{
			RequestValue = specificRecord;
		}

		public T RequestValue { get; }
	}
}
