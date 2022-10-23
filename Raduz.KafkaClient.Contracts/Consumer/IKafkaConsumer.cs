using Avro.Specific;
using Raduz.KafkaClient.Contracts.Requests;

namespace Raduz.KafkaClient.Contracts.Consumer
{
	/// <summary>
	/// Kafka consumer interface
	/// </summary>
	public interface IKafkaConsumer
	{
		/// <summary>
		/// Topic name
		/// </summary>
		string TopicName { get; }

		/// <summary>
		/// AVRO schema type
		/// </summary>
		Type Schema { get; }

		/// <summary>
		/// Create instance of Requst with provided data
		/// </summary>
		/// <param name="data">Data for request ctor</param>
		/// <returns></returns>
		IKafkaClientRequest GetRequest(ISpecificRecord data);
	}
}
