using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mylabel.MobileId.Aggregator.Db;

namespace mylabel.MobileId.Aggregator.Admin.Model
{
	public class ServiceProviderForm
	{
		public ServiceProviderForm() { }
		public ServiceProviderForm(ServiceProvider sp)
		{			
			this.AllowedNotificationUris = sp.AllowedNotificationUris;
			this.AllowedRedirectUris = sp.AllowedRedirectUris;			
			this.ClientId = sp.ClientIdOnAggregator;
			this.ClientName = sp.Name;			
			this.ClientSecretHash = sp.AggregatorClientSecretHash;
			this.Id = sp.Id;
			this.IsInactive = sp.IsInactive;
			this.IsPremiumInfoSigned = sp.IsPremiumInfoSigned;
			this.JwksCachingInSeconds = sp.JwksCachingInSeconds;
			this.JwksValue = sp.JwksValue;			
			this.UseStoredJwksValue = sp.UseStoredJwksValue;					
		}

		public int Id { get; set; }

		[Required]
		public string? ClientName { get; set; }

		[Required]
		public string? ClientId { get; set; }

		public string? ClientSecretHash { get; set; }		

		public string? JwksValue { get; set; }

		public bool UseStoredJwksValue { get; set; }

		public int? JwksCachingInSeconds { get; set; }

		public bool IsPremiumInfoSigned { get; set; }

		public bool IsInactive { get; set; }

		public List<SPNotificationUri>? AllowedNotificationUris { get; set; }

		public List<SPRedirectUri>? AllowedRedirectUris { get; set; }
	}
}
