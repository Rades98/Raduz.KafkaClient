  ____      _    ____  _   _ _____  _  __      __ _          ____ _ _            _     ____        _     _ _     _               
 |  _ \    / \  |  _ \| | | |__  / | |/ /__ _ / _| | ____ _ / ___| (_) ___ _ __ | |_  |  _ \ _   _| |__ | (_)___| |__   ___ _ __ 
 | |_) |  / _ \ | | | | | | | / /  | ' // _` | |_| |/ / _` | |   | | |/ _ \ '_ \| __| | |_) | | | | '_ \| | / __| '_ \ / _ \ '__|
 |  _ <  / ___ \| |_| | |_| |/ /_ _| . \ (_| |  _|   < (_| | |___| | |  __/ | | | |_ _|  __/| |_| | |_) | | \__ \ | | |  __/ |   
 |_| \_\/_/   \_\____/ \___//____(_)_|\_\__,_|_| |_|\_\__,_|\____|_|_|\___|_| |_|\__(_)_|    \__,_|_.__/|_|_|___/_| |_|\___|_|   

How to use:
1) Add settings to your appsettings.json:
	KafkaClientProducerConfig 
		- inherits from https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ProducerConfig.html
		+ AllowCreateTopic - create topic, if there is non while publishing

	SchemaRegistryConfig https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.SchemaRegistry.SchemaRegistryConfig.html

2) Register ConfigureKafkaConsumer and ConfigureKafkaPublisher to services container
	
	Publisher is used like:

	var publisher = app.Services.GetService<IKafkaPublisher>()!; // or some other way to obtain
	await publisher.PublishAsync("{TOPIC-NAME}", "{SOME-KEY}", {YOUR-AVRO-OBJECT}, cancellationToken);

3) Implement Exception handlers
	
	For Publisher implement IPublisherExceptionHandler 

4) Enjoy easily used package :)

Issues or feature requests report here, please https://github.com/Rades98/Raduz.KafkaClient/issues