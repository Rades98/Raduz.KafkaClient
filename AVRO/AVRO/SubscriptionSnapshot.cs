// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by AvroDotnet, version 1.11.1
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace KafkaObjects.AVRO.SubscriptionSnapshot
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	/// <summary>
	/// Subscription Snapshot
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("AvroDotnet", "1.11.1")]
	public partial class SubscriptionSnapshot : global::Avro.Specific.ISpecificRecord
	{
		public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse(@"{""type"":""record"",""name"":""SubscriptionSnapshot"",""doc"":""Subscription Snapshot"",""namespace"":""KafkaObjects.AVRO.SubscriptionSnapshot"",""fields"":[{""name"":""UserId"",""doc"":""User id"",""type"":[""int""]},{""name"":""SalesAndSpecOffers"",""doc"":""Sales and special offers flag"",""type"":[""boolean""]},{""name"":""OffersForOrders"",""doc"":""Offers for orders flag"",""type"":[""boolean""]},{""name"":""GoodsRating"",""doc"":""Goods rating flag"",""type"":[""boolean""]},{""name"":""SatisfactionQuestionnaire"",""doc"":""Satisfaction questionnaire flag"",""type"":[""boolean""]}]}");
		/// <summary>
		/// User id
		/// </summary>
		private object _UserId;
		/// <summary>
		/// Sales and special offers flag
		/// </summary>
		private object _SalesAndSpecOffers;
		/// <summary>
		/// Offers for orders flag
		/// </summary>
		private object _OffersForOrders;
		/// <summary>
		/// Goods rating flag
		/// </summary>
		private object _GoodsRating;
		/// <summary>
		/// Satisfaction questionnaire flag
		/// </summary>
		private object _SatisfactionQuestionnaire;
		public virtual global::Avro.Schema Schema
		{
			get
			{
				return SubscriptionSnapshot._SCHEMA;
			}
		}
		/// <summary>
		/// User id
		/// </summary>
		public object UserId
		{
			get
			{
				return this._UserId;
			}
			set
			{
				this._UserId = value;
			}
		}
		/// <summary>
		/// Sales and special offers flag
		/// </summary>
		public object SalesAndSpecOffers
		{
			get
			{
				return this._SalesAndSpecOffers;
			}
			set
			{
				this._SalesAndSpecOffers = value;
			}
		}
		/// <summary>
		/// Offers for orders flag
		/// </summary>
		public object OffersForOrders
		{
			get
			{
				return this._OffersForOrders;
			}
			set
			{
				this._OffersForOrders = value;
			}
		}
		/// <summary>
		/// Goods rating flag
		/// </summary>
		public object GoodsRating
		{
			get
			{
				return this._GoodsRating;
			}
			set
			{
				this._GoodsRating = value;
			}
		}
		/// <summary>
		/// Satisfaction questionnaire flag
		/// </summary>
		public object SatisfactionQuestionnaire
		{
			get
			{
				return this._SatisfactionQuestionnaire;
			}
			set
			{
				this._SatisfactionQuestionnaire = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.UserId;
			case 1: return this.SalesAndSpecOffers;
			case 2: return this.OffersForOrders;
			case 3: return this.GoodsRating;
			case 4: return this.SatisfactionQuestionnaire;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.UserId = (System.Object)fieldValue; break;
			case 1: this.SalesAndSpecOffers = (System.Object)fieldValue; break;
			case 2: this.OffersForOrders = (System.Object)fieldValue; break;
			case 3: this.GoodsRating = (System.Object)fieldValue; break;
			case 4: this.SatisfactionQuestionnaire = (System.Object)fieldValue; break;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}