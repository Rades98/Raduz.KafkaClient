using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raduz.KafkaClient.Contracts.Publisher;

namespace Raduz.KafkaClient.Publisher
{
	public static class DependencyRegistrations
	{
		public static IServiceCollection ConfigureKafkaPublisher(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<ProducerConfig>(configuration.GetSection(nameof(ProducerConfig)));
			services.AddScoped<IKafkaPublisher, KafkaPublisher>();

			return services;
		}
	}
}