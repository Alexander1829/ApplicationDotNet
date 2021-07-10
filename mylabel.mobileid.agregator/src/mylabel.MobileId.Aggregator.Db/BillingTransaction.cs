using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mylabel.MobileId.Aggregator.Db
{
	[Table("BillingTransactions")]
	public class BillingTransaction
	{
		public int Id { get; set; }

		[Required]
		public string? record_type { get; set; }

		[Required]
		public string? subscriber_no { get; set; }

		[Required]
		public DateTimeOffset? channel_seizure_date_time { get; set; }

		[Required]
		public string? message_switch_id { get; set; }

		[Required]
		public string? at_feature_code { get; set; }

		[Required]
		public string? call_action_code { get; set; }

		[Required]
		public string? call_to_tn_sgsn { get; set; }

		[Required]
		public string? Calling_no_ggsn { get; set; }

		[Required]
		public string? guide_by { get; set; }

		[Required]
		public int? data_volume { get; set; }

		[Required]
		public string? call_source { get; set; }

		[Required]
		public string? basic_service_code { get; set; }

		[Required]
		public string? basic_service_type { get; set; }

		[Required]
		public string? uom { get; set; }

		[Required]
		public string? technology { get; set; }

		[Required]
		public string? call_destination { get; set; }

		[Required][RegularExpression(@"DI|SI")]
		public string? SiOrDi { get; set; }

		[Required]
		public long? AuthorizationId { get; set; }
	}
}
