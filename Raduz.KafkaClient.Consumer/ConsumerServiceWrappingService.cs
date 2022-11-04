using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Raduz.KafkaClient.Consumer
{
	public class ConsumerServiceWrappingService : BackgroundService
	{
		private readonly IServiceProvider _services;
		private IServiceScope _scope;

		public ConsumerServiceWrappingService(IServiceProvider services)
		{
			_services = services ?? throw new ArgumentNullException(nameof(services));
			_scope = _services.CreateScope();
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var scopedProcessingService =
				_scope.ServiceProvider
					.GetRequiredService<IConsumingService>();

			await scopedProcessingService.DoWork(stoppingToken);
		}

		public override void Dispose()
		{
			_scope.Dispose();
			base.Dispose();
		}
	}
}
