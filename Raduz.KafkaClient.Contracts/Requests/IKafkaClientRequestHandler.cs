using MediatR;

namespace Raduz.KafkaClient.Contracts.Requests
{
	/// <summary>
	/// IKafkaClientRequestHandler
	/// </summary>
	/// <typeparam name="TRequest"><seealso cref="IKafkaClientRequest"/></typeparam>
	public interface IKafkaClientRequestHandler<TRequest> : IRequestHandler<TRequest, bool> where TRequest : IKafkaClientRequest
	{
	}
}
