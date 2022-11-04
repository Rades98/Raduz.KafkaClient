  ____      _    ____  _   _ _____  _  __      __ _          ____ _ _            _     ____            _                  _       
 |  _ \    / \  |  _ \| | | |__  / | |/ /__ _ / _| | ____ _ / ___| (_) ___ _ __ | |_  / ___|___  _ __ | |_ _ __ __ _  ___| |_ ___ 
 | |_) |  / _ \ | | | | | | | / /  | ' // _` | |_| |/ / _` | |   | | |/ _ \ '_ \| __|| |   / _ \| '_ \| __| '__/ _` |/ __| __/ __|
 |  _ <  / ___ \| |_| | |_| |/ /_ _| . \ (_| |  _|   < (_| | |___| | |  __/ | | | |_ | |__| (_) | | | | |_| | | (_| | (__| |_\__ \
 |_| \_\/_/   \_\____/ \___//____(_)_|\_\__,_|_| |_|\_\__,_|\____|_|_|\___|_| |_|\__(_)____\___/|_| |_|\__|_|  \__,_|\___|\__|___/


 THIS PACKAGE IS ONLY ABSTRACTIONS.. IT WON'T WORK WITHOUT 
 Raduz.KafkaClient.Client, Raduz.KafkaClient.Consumer or  Raduz.KafkaClient.Publisher


 How to use:
1) Add settings to your appsettings.json:
	KafkaClientConsumerConfig 
		- inherits from https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ConsumerConfig.html
		+ MaxConsumeRetryCount count of retries (retry pattern)

	KafkaClientProducerConfig 
		- inherits from https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ProducerConfig.html
		+ AllowCreateTopic - create topic, if there is non while publishing

	SchemaRegistryConfig https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.SchemaRegistry.SchemaRegistryConfig.html

2) Register ConfigureKafkaConsumer and ConfigureKafkaPublisher to services container
	
	Publisher is used like:

	var publisher = app.Services.GetService<IKafkaPublisher>()!; // or some other way to obtain
	await publisher.PublishAsync("{TOPIC-NAME}", "{SOME-KEY}", {YOUR-AVRO-OBJECT}, cancellationToken);


	For Consumers create handlers for each topic like: 

	public class YourRequest : KafkaConsumerHandler<{YOUR-AVRO-OBJECT}>
	{
		public YourRequest() : base("{TOPIC-NAME}")
		{
		}
	}

	In base class is injected IConsumerManager which is singleton managing pause/resume actions for soncumer,
	so if there is error with your DB f.e. you can pause consuming of all topics

3) Implement Exception handlers
	
	For Publisher implement IPublisherExceptionHandler 

	For Consumer implement IConsumerExceptionHandler

4) Enjoy easily used package :)

Issues or feature requests report here, please https://github.com/Rades98/Raduz.KafkaClient/issues