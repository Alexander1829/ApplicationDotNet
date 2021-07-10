using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mylabel.MobileId.Aggregator.Db
{
	[Table("PremiumInfoTokens")]
	public class PremiumInfoToken
	{
		public int Id { get; set; }

		[Required]
		public string? AccessTokenOnAggregatorHash { get; set; }

		[Required]
		public string? AccessTokenOnIdgw { get; set; }
				
		public DateTimeOffset CreatedAt { get; set; }

		[Required][RegularExpression(@"DI|SI")]
		public string? AfterSiOrDi { get; set; }
	}
}
