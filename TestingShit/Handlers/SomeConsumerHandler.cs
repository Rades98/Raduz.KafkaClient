using KafkaObjects.AVRO.SubscriptionSnapshot;
using Raduz.KafkaClient.Contracts.Requests;

namespace TestingShit.Handlers
{
	public class SomeConsumerHandler : KafkaConsumerHandler<SubscriptionSnapshot>
	{
		public SomeConsumerHandler() : base("subscriptionservice-snapshot")
		{
		}

		public override Task<bool> HandleAsync(SubscriptionSnapshot record, CancellationToken ct)
		{
			Console.WriteLine($"obtained daata of user {record.UserId}\n" +
				$"Goods rating: {record.GoodsRating}\n" +
				$"OffersForOrders: {record.OffersForOrders}\n" +
				$"SalesAndSpecOffers: {record.SalesAndSpecOffers}\n" +
				$"SatisfactionQuestionnaire: {record.SatisfactionQuestionnaire}\n\n");

			return Task.FromResult(true);
		}
	}
}
