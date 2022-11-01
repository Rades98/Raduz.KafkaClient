![logo](https://user-images.githubusercontent.com/37796889/197366999-7aef1ddf-8bae-4859-b40f-3d0d9c542df4.png)

# Raduz.KafkaClient
Easy to use kafka .net6 client.
+ publishing and consuming AVRO schema based topics easilly with mediator request and handlers
+ some logs of operations and their state

# How to use
You can download packages from [here](https://github.com/Rades98?tab=packages) 
Or just install from nuget.org:
+ NuGet\Install-Package Raduz.KafkaClient.Client
  - Universall with all of bellow
+ NuGet\Install-Package Raduz.KafkaClient.Common
  - Some extensions
+ NuGet\Install-Package Raduz.KafkaClient.Consumer
  - Can be used as solo consumer
+ NuGet\Install-Package Raduz.KafkaClient.Contracts
  - Can be used as microsoft "Abstractions" packages in projects,</br> where is no need to include whole package, </br> but you need contracts for building requests or some pipelines etc.
+ NuGet\Install-Package Raduz.KafkaClient.Publisher
  - Can be used as solo publisher

## AVRO object creation
How to create AVROModel you can read [here](https://engineering.chrobinson.com/dotnet-avro/guides/cli-generate/)
+ !Dont forget to add its avsc file to your schema registry!

## Registration
To use kafka client you have to register it in your DI container and provide configuration

### appsettings.json
In app settings you can set consumer, producer and schema registry (at this point there is no common registration). </br>All registration settings are made by Confluent.Kafka. You can read more in their documentation 
+ [ConsumerConfig](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ConsumerConfig.html)
+ [ProducerrConfig](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ProducerConfig.html)
+ [SchemaRegistryConfig](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.SchemaRegistry.SchemaRegistryConfig.html)

``` json
"KafkaClientConsumerConfig": {
    "GroupId": "{CONSUMER-GROUP-ID}",
    "BootstrapServers": "{YOUR-BOOTSTRAP-KAFKA-SERVER}",
    "AutoOffsetReset" : 1,
    //OPTIONAL f.e.
    "SecurityProtocol": 3,
    "SaslMechanism": {SASL-MECHANISM},
    "SaslUsername": "{SASL-USER-NAME}",
    "SaslPassword": "{SASL-USER-PASSWORD}"
  },
  "SchemaRegistryConfig": {
    "Url": "{SCHEMA-REGISTRY-URL}",
    //OPTIONAL f.e.
    "BasicAuthUserInfo": "{KEY}:{TOKEN}"
  },
  "KafkaClientProducerConfig": {
    "BootstrapServers": "{YOUR-BOOTSTRAP-KAFKA-SERVER}",
    //OPTIONAL f.e.
    "SecurityProtocol": 3,
    "SaslMechanism": {SASL-MECHANISM},
    "SaslUsername": "{SASL-USER-NAME}",
    "SaslPassword": "{SASL-USER-PASSWORD}"
  }
```
### Configure services
Add this code snipet to your service registration.
``` cs
   services.ConfigureKafkaPublisher(configuration)
      .ConfigureKafkaConsumer(configuration);
```
## Consumer
Here we need to create handler implementing kafka client interfaces and abstract class

### Handler creation
Creating handler is realy easy, cause only thing that you need is just to create class implementing abstract class KafkaConsumerHandler and provide some generated object from your AVRO scheme and topic name. All Handlers are registered automagically, so you don't have to do any actions. Just create handler for topic and its avro object implementing ISpecificRecord.
``` cs
public class YourRequest : KafkaConsumerHandler<{YOUR-AVRO-OBJECT}>
{
  public YourRequest() : base("{TOPIC-NAME}")
  {
  }
}
```  

### Exception handling
For handling exceptions you can implement IConsumerExceptionHandler
```  cs
public class ConsumerExceptionHandler : IConsumerExceptionHandler
{
  private readonly ILogger _logger;
  public ConsumerExceptionHandler(ILogger logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public Task Handle(Exception? e)
  {
    _logger.LogError(e);
  }
}
``` 

### Pipeline creation
For logging or performing some other stuff on consumming you can write your very own pipelines by creating objects implementing IConsumerPipelineBehaviour

```  cs
public class ConsumerPipeline : IConsumerPipelineBehaviour
{
  private readonly ILogger _logger;
  public ConsumerPipeline(ILogger logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async Task<bool> Handle(HandlerDelegate next, ISpecificRecord data, CancellationToken ct)
  {
    var sw = new Stopwatch();
    sw.Start();
    var result = await next();
    sw.Stop();
    
    _logger.LogInformation("Request handling took {elapsed} ms", sw.ElapsedMilliseconds); 

    return result;
  }
}
``` 

## Publisher
To publish record to Kafka there is nothing easier than injecting IKafkaPusher to your class and call like:
```  cs
var publisher = app.Services.GetService<IKafkaPublisher>()!; // or some other way to obtain
await publisher.PublishAsync("{TOPIC-NAME}", "{SOME-KEY}", {YOUR-AVRO-OBJECT}, cancellationToken);
``` 
### Exception handling
For handling exceptions you can implement IPublisherExceptionHandler
```  cs
public class PublisherExceptionHandler : IPublisherExceptionHandler
{
  private readonly ILogger _logger;
  public ConsumerExceptionHandler(ILogger logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public Task Handle(Exception? e)
  {
    _logger.LogError(e);
  }
}
``` 

# Used nuggets
Project 'Raduz.KafkaClient.Contracts' has the following package references
   > Confluent.SchemaRegistry.Serdes.Avro      1.9.3 

Project 'Raduz.KafkaClient.Consumer' has the following package references
   > Microsoft.Extensions.Configuration.Abstractions           6.0.0 
   > Microsoft.Extensions.Hosting.Abstractions                 6.0.0  
   > Microsoft.Extensions.Options.ConfigurationExtensions      6.0.0   
   > YCherkes.SchemaRegistry.Serdes.Avro                       1.0.4 

Project 'Raduz.KafkaClient.Publisher' has the following package references
   > Microsoft.Extensions.Configuration.Abstractions            6.0.0 
   > Microsoft.Extensions.DependencyInjection.Abstractions      6.0.0 
   > Microsoft.Extensions.Logging.Abstractions                  6.0.2 
   > Microsoft.Extensions.Options.ConfigurationExtensions       6.0.0   

Project 'Raduz.KafkaClient.Common' has the following package references
   > Confluent.Kafka                                1.9.3 
   > Microsoft.Extensions.Logging.Abstractions      6.0.2  
