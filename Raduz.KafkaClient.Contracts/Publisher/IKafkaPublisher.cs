using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avro.Specific;

namespace Raduz.KafkaClient.Contracts.Publisher
{
	public interface IKafkaPublisher
	{
		Task PublishAsync<T>(string topicName, string key, T data, CancellationToken ct) where T : class, ISpecificRecord;
	}
}
