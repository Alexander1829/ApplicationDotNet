using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mylabel.MobileId.Aggregator.Db
{
	[Table("ServiceProviders"), Index(nameof(ClientIdOnAggregator), IsUnique = true)]
	public class ServiceProvider
	{
		public int Id { get; set; }

		[Required]
		public string? Name { get; set; }

		[Required]
		public string? ClientIdOnAggregator { get; set; }

		[Required]
		public string? AggregatorClientSecretHash { get; set; }

		public string? JwksUri { get; set; }

		public string? JwksValue { get; set; }

		public bool UseStoredJwksValue { get; set; }

		public int? JwksCachingInSeconds { get; set; }

		public bool IsPremiumInfoSigned { get; set; }

		public long BillingCtn { get; set; }

		public bool IsInactive { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public string? AllowedIPAddresses { get; set; }

		public List<SPNotificationUri>/*?*/ AllowedNotificationUris { get; set; } = new();

		public List<SPRedirectUri>/*?*/ AllowedRedirectUris { get; set; } = new();

		public List<SPToDiscoveryLink>/*?*/ Discoveries { get; set; } = new();
	}
}