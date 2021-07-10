using System;

namespace mylabel.MobileId.Aggregator.Admin.Model
{
	public class AuthorizationRequest
	{
		public string ClientIdOnAggregator { get; set; }
		public string Mode { get; set; }
		public string? ServingOperator { get; set; }
		public string Status { get; set; }
		public DateTimeOffset ExecutedAt { get; set; }
	}
}
