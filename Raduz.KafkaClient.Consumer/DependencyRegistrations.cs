using Confluent.Kafka;
using Confluent.SchemaRegistry;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raduz.KafkaClient.Consumer.Consumer;

namespace Raduz.KafkaClient.Consumer
{
	public static class DependencyRegistrations
	{
		public static IServiceCollection ConfigureKafkaConsumer(this IServiceCollection services, IConfiguration configuration, Action<KafkaConsumerCollections> opts)
		{
			services.Configure<ConsumerConfig>(configuration.GetSection(nameof(ConsumerConfig)));
			services.Configure<SchemaRegistryConfig>(configuration.GetSection(nameof(SchemaRegistryConfig)));
			services.AddHostedService<ConsumingService>();

			var kafkaOpts = new KafkaConsumerCollections();
			opts(kafkaOpts);

			services.AddSingleton(kafkaOpts);

			services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

			return services;
		}
	}
}