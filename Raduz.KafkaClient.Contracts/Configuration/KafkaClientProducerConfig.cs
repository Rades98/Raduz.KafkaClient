using Confluent.Kafka;

namespace Raduz.KafkaClient.Contracts.Configuration
{
	public class KafkaClientProducerConfig : ProducerConfig
	{
		public bool AllowCreateTopic { get; set; }
	}
}
