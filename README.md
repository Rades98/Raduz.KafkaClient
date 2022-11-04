![logo](https://user-images.githubusercontent.com/37796889/197366999-7aef1ddf-8bae-4859-b40f-3d0d9c542df4.png)

# Raduz.KafkaClient
Easy to use kafka .net6 client.
+ publishing and consuming AVRO schema based topics easilly with mediator request and handlers
+ some logs of operations and their state

# How to use

## For more specific info about usage chceck [this project's wiki](https://github.com/Rades98/Raduz.KafkaClient/wiki)

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
