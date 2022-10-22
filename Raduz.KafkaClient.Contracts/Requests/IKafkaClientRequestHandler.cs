using Avro.Specific;
using MediatR;

namespace Raduz.KafkaClient.Contracts.Requests
{
	public interface IKafkaClientRequestHandler<TRequest> : IRequestHandler<TRequest, bool> where TRequest : IKafkaClientRequest
	{
	}
}
