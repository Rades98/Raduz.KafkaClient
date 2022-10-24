using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raduz.KafkaClient.Consumer;
using Raduz.KafkaClient.Publisher;
using Serilog;

var builder = new ConfigurationBuilder()
	.SetBasePath(Path.Combine(AppContext.BaseDirectory))
	.AddJsonFile("appsettings.json", optional: true);

var configuration = builder.Build();

var host = Host.CreateDefaultBuilder()
	.ConfigureServices(services =>
	{
		services
			.AddLogging(builder => builder.AddSerilog())

			//Kafka
			.ConfigureKafkaPublisher(configuration)
			.ConfigureKafkaConsumer(configuration, Assembly.GetExecutingAssembly())
			.BuildServiceProvider();
	});

var app = host.Build();

app.Run();