  ____      _    ____  _   _ _____  _  __      __ _          ____ _ _            _     ____                                          
 |  _ \    / \  |  _ \| | | |__  / | |/ /__ _ / _| | ____ _ / ___| (_) ___ _ __ | |_  / ___|___  _ __  ___ _   _ _ __ ___   ___ _ __ 
 | |_) |  / _ \ | | | | | | | / /  | ' // _` | |_| |/ / _` | |   | | |/ _ \ '_ \| __|| |   / _ \| '_ \/ __| | | | '_ ` _ \ / _ \ '__|
 |  _ <  / ___ \| |_| | |_| |/ /_ _| . \ (_| |  _|   < (_| | |___| | |  __/ | | | |_ | |__| (_) | | | \__ \ |_| | | | | | |  __/ |   
 |_| \_\/_/   \_\____/ \___//____(_)_|\_\__,_|_| |_|\_\__,_|\____|_|_|\___|_| |_|\__(_)____\___/|_| |_|___/\__,_|_| |_| |_|\___|_|   


How to use:
1) Add settings to your appsettings.json:
	KafkaClientConsumerConfig 
		- inherits from https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ConsumerConfig.html
		+ MaxConsumeRetryCount count of retries (retry pattern)


	SchemaRegistryConfig https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.SchemaRegistry.SchemaRegistryConfig.html

2) Register ConfigureKafkaConsumer and ConfigureKafkaPublisher to services container
	
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
	
	For Consumer implement IConsumerExceptionHandler

4) Implement consumer pipeline behaviour

	IConsumerPipelineBehaviour 
	

5) Enjoy easily used package :)

Issues or feature requests report here, please https://github.com/Rades98/Raduz.KafkaClient/issues