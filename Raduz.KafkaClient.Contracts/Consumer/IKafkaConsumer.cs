﻿using Avro.Specific;
using Raduz.KafkaClient.Contracts.Requests;

namespace Raduz.KafkaClient.Contracts.Consumer
{
	public interface IKafkaConsumer
	{
		string TopicName { get; }
		Type Schema { get; }

		IKafkaClientRequest GetRequest(ISpecificRecord data);
	}
}
