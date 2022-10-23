# Raduz.KafkaClient
Easy to use kafka .net6 client.
+ publishing and consuming AVRO schema based topics easilly with mediator request and handlers
+ some logs of operations and their state

# How to use
You can download packages from [here](https://github.com/Rades98?tab=packages) 

## AVRO object creation
How to create AVROModel you can read [here](https://engineering.chrobinson.com/dotnet-avro/guides/cli-generate/)
+ !Dont forget to add its avsc file to your schema registry!

## Registration
To use kafka client you have to register it in your DI container and provide configuration

### appsettings.json
In app settings you can set consumer, producer and schema registry (at this point there is no common registration)
``` json
"ConsumerConfig": {
    "GroupId": "{CONSUMER-GROUP-ID}",
    "BootstrapServers": "{YOUR-BOOTSTRAP-KAFKA-SERVER}",
    "AutoOffsetReset" : 1,
    //OPTIONAL
    "SecurityProtocol": 3,
    "SaslMechanism": {SASL-MECHANISM},
    "SaslUsername": "{SASL-USER-NAME}",
    "SaslPassword": "{SASL-USER-PASSWORD}"
  },
  "SchemaRegistryConfig": {
    "Url": "{SCHEMA-REGISTRY-URL}",
    //OPTIONAL
    "BasicAuthUserInfo": "{KEY}:{TOKEN}"
  },
  "ProducerConfig": {
    "BootstrapServers": "{YOUR-BOOTSTRAP-KAFKA-SERVER}",
    //OPTIONAL
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
      .ConfigureKafkaConsumer(configuration, opts =>
      {
        //Here you can register consumer requests for specific topic and AVRO model
        opts.AddConsumer<{AVRO-SCHEME}, {MEDIATOR-REQUEST}>("{TOPIC-NAME}");
        ...
      })
```
## Consumer
Here we need to create request and handler implementing kafka client interfaces and abstract class

### Request creation
Creating request is realy easy, cause only thing that you need is just to create class implementing abstract class KafkaClientRequest and provide some generated object from your AVRO scheme
``` cs
public class YourRequest : KafkaClientRequest<AVROModel>
{
  public YourRequest(AVROModel specificRecord) : base(specificRecord)
  {
  }
}
```  

### Handler creation
Handler should implement IKafkaClientRequestHandler where the generic argument is your created request
```  cs
public class YourHandler : IKafkaClientRequestHandler<YourRequest>
{
  public YourHandler()
  {
    //YOUR DI
  }

  public async Task<bool> Handle(BulkCreateSubscriptionRequest request, CancellationToken cancellationToken)
  {
    //Handle request
    if ({HANDLED})
    {
      return true;
    }

    return false;
  }
}
```  

## Publisher
To publish record to Kafka there is nothing easier than injecting IKafkaPusher to your class and call like:
```  cs
var publisher = app.Services.GetService<IKafkaPublisher>()!; // or some other way to obtain
await publisher.PublishAsync("{TOPIC-NAME}", "{SOME-KEY}", new AVROModel() { Data = data }, cancellationToken);
``` 

# Used nuggets
Project 'Raduz.KafkaClient.Contracts' has the following package references
   > Confluent.SchemaRegistry.Serdes.Avro      1.9.3 
   > MediatR                                   11.0.0

Project 'Raduz.KafkaClient.Consumer' has the following package references
   > MediatR.Extensions.Microsoft.DependencyInjection          11.0.0 
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
