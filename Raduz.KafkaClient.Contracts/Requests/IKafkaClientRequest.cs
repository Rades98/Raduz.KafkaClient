using MediatR;

namespace Raduz.KafkaClient.Contracts.Requests
{
	/// <summary>
	/// Kafka client request inerface
	/// </summary>
	public interface IKafkaClientRequest : IRequest<bool>
	{
	}
}
