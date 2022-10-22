using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raduz.KafkaClient.Contracts.Publisher;

namespace Raduz.KafkaClient.Publisher
{
	public static class DependencyRegistrations
	{
		/// <summary>
		/// ConfigureKafkaPublisher
		/// </summary>
		/// <param name="services">Service collection of your DI container</param>
		/// <param name="configuration">Configuration from app settings </param>
		/// <returns></returns>
		public static IServiceCollection ConfigureKafkaPublisher(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<ProducerConfig>(configuration.GetSection(nameof(ProducerConfig)));
			services.AddScoped<IKafkaPublisher, KafkaPublisher>();

			return services;
		}
	}
}