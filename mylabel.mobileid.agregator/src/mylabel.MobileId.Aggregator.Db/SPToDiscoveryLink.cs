using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mylabel.MobileId.Aggregator.Db
{
	[Table("SPToDiscoveryLinks")]
	[Index(nameof(ServiceProviderId), nameof(DiscoveryServiceId), IsUnique = true)]
	public class SPToDiscoveryLink
	{
		public int Id { get; set; }

		public int ServiceProviderId { get; set; }

		public int DiscoveryServiceId { get; set; }

		public DiscoveryService? DiscoveryService { get; set; }

		[Required]
		public string? ClientIdOnDiscovery { get; set; }

		[Required]
		public string? ClientSecretOnDiscovery { get; set; }

		[Required]
		public string? RedirectUri { get; set; }

		public bool IsEnabled { get; set; }
	}
}