using System;

namespace mylabel.MobileId.Aggregator.Admin.Model
{
	public class AuthorizeReportRequest
	{
		public DateTimeOffset? DateFrom { get; set; }
		public DateTimeOffset? DateBefore { get; set; }
	}
}
