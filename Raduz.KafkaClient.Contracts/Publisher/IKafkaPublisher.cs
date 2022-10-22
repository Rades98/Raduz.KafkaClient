using Avro.Specific;

namespace Raduz.KafkaClient.Contracts.Publisher
{
	/// <summary>
	/// KAfka publisher interface
	/// </summary>
	public interface IKafkaPublisher
	{
		/// <summary>
		/// Publish AVRO message async
		/// </summary>
		/// <typeparam name="T">AVRO <seealso cref="ISpecificRecord"/></typeparam>
		/// <param name="topicName">Topic name</param>
		/// <param name="key">kafka message key</param>
		/// <param name="data">AVRO scheme data</param>
		/// <param name="ct">Cancellation token</param>
		/// <returns></returns>
		Task PublishAsync<T>(string topicName, string key, T data, CancellationToken ct) where T : class, ISpecificRecord;
	}
}
