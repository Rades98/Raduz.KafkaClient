using Avro.Specific;
using Raduz.KafkaClient.Contracts.Consumer;
using Raduz.KafkaClient.Contracts.Requests;

namespace Raduz.KafkaClient.Consumer.Consumer
{
	public class KafkaConsumerCollections
	{
		private List<IKafkaConsumer> _consumerRegistrations = new();
		private Dictionary<string, IKafkaConsumer>? _consumerRegistrationsDictionary;

		public Dictionary<string, IKafkaConsumer> ConsumerRegistrations => GetDictionary();

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
