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
		/// <summary>
		/// Kafka client configuration
		/// </summary>
		/// <param name="services">Service collection of your DI container</param>
		/// <param name="configuration">Configuration from app settings </param>
		/// <param name="opts">Kafka Consumer collection <seealso cref="KafkaConsumerCollection"/></param>
		/// <returns></returns>
		public static IServiceCollection ConfigureKafkaConsumer(this IServiceCollection services, IConfiguration configuration, Action<KafkaConsumerCollection> opts)
		{
			services.Configure<ConsumerConfig>(configuration.GetSection(nameof(ConsumerConfig)));
			services.Configure<SchemaRegistryConfig>(configuration.GetSection(nameof(SchemaRegistryConfig)));
			services.AddHostedService<ConsumingService>();

			var kafkaOpts = new KafkaConsumerCollection();
			opts(kafkaOpts);

			services.AddSingleton(kafkaOpts);

			services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

			return services;
		}
	}
}