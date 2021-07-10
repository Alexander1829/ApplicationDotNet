using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mylabel.MobileId.Aggregator.Db
{
	[Table("SIAuthorizationRequests")]
	[Index(nameof(AuthorizationRequestId), IsUnique = true)]
	public class SIAuthorizationRequest
	{
		public long Id { get; set; }

		[Required]
		public string? SPNotificationUri { get; set; }

		[Required]
		public string? SPNotificationToken { get; set; }

		public string? SPNonce { get; set; }

		[Required]
		public string? ClientIdOnAggregator { get; set; }

		[Required]
		public string? IdgwJwksUri { get; set; }

		public Guid AuthorizationRequestId { get; set; }

		[Required]
		public string? AggregatorNotificationToken { get; set; }

		public Guid AggregatorNonce { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset? RespondedAt { get; set; }

		public string? ServingOperator { get; set; }

		public string? PremiumInfoEndpoint { get; set; }

		public string? AccessTokenOnAggregatorHash { get; set; }

		[Required]
		public string? Msisdn { get; set; }

		[Required]
		public string? Scope { get; set; }

		[Required]
		public string? AcrValues { get; set; }

		[Required]
		public bool BillingSuccess { get; set; }
	}
}