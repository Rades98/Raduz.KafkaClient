using Confluent.Kafka;

namespace Raduz.KafkaClient.Contracts.Configuration
{
	public class KafkaClientConsumerConfig : ConsumerConfig
	{
		public int MaxConsumeRetryCount { get; set; }
	}
}
