using System.Reflection;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raduz.KafkaClient.Contracts.Consumer;
using Raduz.KafkaClient.Contracts.Consumer.Handler;

namespace Raduz.KafkaClient.Consumer
{
	public static class DependencyRegistrations
	{
		/// <summary>
		/// Kafka client configuration
		/// </summary>
		/// <param name="services">Service collection of your DI container</param>
		/// <param name="configuration">Configuration from app settings </param>
		/// <returns></returns>
		public static IServiceCollection ConfigureKafkaConsumer(
			this IServiceCollection services,
			IConfiguration configuration, Assembly assembly)
		{
			services.Configure<ConsumerConfig>(configuration.GetSection(nameof(ConsumerConfig)));
			services.Configure<SchemaRegistryConfig>(configuration.GetSection(nameof(SchemaRegistryConfig)));
			services.AddHostedService<ConsumingService>();

			assembly.GetTypes().Where(type =>
				type.BaseType is not null &&
				type.BaseType.IsGenericType &&
				type.BaseType.GetGenericTypeDefinition() is not null &&
				ReferenceEquals(type.BaseType.GetGenericTypeDefinition(), typeof(KafkaConsumerHandler<>)) &&
				type.BaseType.GetGenericArguments() is not null &&
				type.BaseType.GetGenericArguments().Length >= 1 &&
				typeof(IKafkaHandler).IsAssignableFrom(type.BaseType)
			).ToList()
			.ForEach(type => services.AddScoped(typeof(IKafkaHandler), type));

			var errorHandler = assembly.GetTypes().FirstOrDefault(type => type.IsAssignableFrom(typeof(IConsumerExceptionHandler)));
			if(errorHandler is not null)
			{
				services.AddScoped(typeof(IConsumerExceptionHandler), errorHandler);
			}

			return services;
		}
	}
}