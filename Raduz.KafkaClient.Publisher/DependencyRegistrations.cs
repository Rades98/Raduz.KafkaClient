using System.Reflection;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
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
			services.Configure<SchemaRegistryConfig>(configuration.GetSection(nameof(SchemaRegistryConfig)));
			services.AddScoped<IKafkaPublisher, KafkaPublisher>();

			return services;
		}

		/// <summary>
		/// Configure Kafka Publisher used when IPublisherExceptionHandler is implemented
		/// </summary>
		/// <param name="services">Service collection of your DI container</param>
		/// <param name="configuration">Configuration from app settings </param>
		/// <param name="assembly">Assembly </param>
		/// <returns></returns>
		public static IServiceCollection ConfigureKafkaPublisher(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
		{
			services.Configure<ProducerConfig>(configuration.GetSection(nameof(ProducerConfig)));
			services.Configure<SchemaRegistryConfig>(configuration.GetSection(nameof(SchemaRegistryConfig)));
			services.AddScoped<IKafkaPublisher, KafkaPublisher>();

			var errorHandler = assembly.GetTypes().FirstOrDefault(type => type.IsAssignableFrom(typeof(IPublisherExceptionHandler)));
			if (errorHandler is not null)
			{
				services.AddScoped(typeof(IPublisherExceptionHandler), errorHandler);
			}

			return services;
		}
	}
}