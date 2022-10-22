using Avro.Specific;
using Raduz.KafkaClient.Contracts.Consumer;
using Raduz.KafkaClient.Contracts.Requests;

namespace Raduz.KafkaClient.Consumer.Consumer
{
	/// <summary>
	/// Kafka consumer collections
	/// </summary>
	public sealed class KafkaConsumerCollection
	{
		private List<IKafkaConsumer> _consumerRegistrations = new();
		private Dictionary<string, IKafkaConsumer>? _consumerRegistrationsDictionary;

		/// <summary>
		/// Dictionary of registered Kafka consumers
		/// </summary>
		public Dictionary<string, IKafkaConsumer> ConsumerRegistrations => GetDictionary();

		/// <summary>
		/// Add kafka consumer
		/// </summary>
		/// <typeparam name="TRecord">AVRO specific record</typeparam>
		/// <typeparam name="TRequest">Kafka client request <seealso cref="KafkaClientRequest"/></typeparam>
		/// <param name="topicName">Topic name</param>
		public void AddConsumer<TRecord, TRequest>(string topicName) where TRecord : ISpecificRecord where TRequest : KafkaClientRequest<TRecord>
		{
			_consumerRegistrations.Add(new KafkaConsumer<TRecord, TRequest>(topicName));
		}

		private Dictionary<string, IKafkaConsumer> GetDictionary()
		{
			if (_consumerRegistrationsDictionary is null)
			{
				_consumerRegistrationsDictionary = _consumerRegistrations.ToDictionary(consReg => consReg.Schema.Name, consReg => consReg);
			}
			return _consumerRegistrationsDictionary;
		}
	}
}
