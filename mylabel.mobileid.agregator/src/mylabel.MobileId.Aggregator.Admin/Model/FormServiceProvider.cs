using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using mylabel.MobileId.Aggregator.Db;
using mylabel.MobileId.Aggregator.Admin.Model.Helper;

namespace mylabel.MobileId.Aggregator.Admin.Model
{
	public class FormServiceProvider
	{
		string? validAggregatorClientSecretHash { get; set; }

		public FormServiceProvider()
		{
			ServiceProvider = new();
		}

		public FormServiceProvider(ServiceProvider sp)
		{
			this.ServiceProvider = sp;

			var enabledDisco = sp.Discoveries?.FirstOrDefault(l => l.IsEnabled);

			this.ValidClientIdOnAggregator = sp.ClientIdOnAggregator;
			this.ValidClientName = sp.Name;
			this.validAggregatorClientSecretHash = sp.AggregatorClientSecretHash;
			this.ValidClientIdOnDiscovery = enabledDisco?.ClientIdOnDiscovery;
			this.ValidClientSecretOnDiscovery = enabledDisco?.ClientSecretOnDiscovery;
			this.ValidDiscoveryServiceId = enabledDisco?.DiscoveryServiceId;
			this.ValidRedirectUriOnDiscovery = enabledDisco?.RedirectUri;
			this.ValidBillingCtn = sp.BillingCtn;
			this.AllowedIPAddresses = sp.AllowedIPAddresses;
			if (sp.Id != 0)
			{
				if (sp.AllowedRedirectUris.Count > 0)
					this.ValidSPRedirectUriFirst = sp.AllowedRedirectUris[0].Value;
				if (sp.AllowedNotificationUris.Count > 0)
					this.ValidSPNotificationUriFirst = sp.AllowedNotificationUris[0].Value;
				if (!string.IsNullOrEmpty(sp.JwksUri) || !string.IsNullOrEmpty(sp.JwksValue))
					this.ValidJwksUriOrValueSetted = true;
				if (!string.IsNullOrEmpty(sp.AggregatorClientSecretHash))
					this.ValidAggregatorClientSecretsAreEquals = true;
			}
		}

		public ServiceProvider ServiceProvider { get; set; }

		[Required(ErrorMessage = "The Client Name is required.")]
		public string? ValidClientName { get; set; }

		[Required(ErrorMessage = "The Client ID is required.")]
		public string? ValidClientIdOnAggregator { get; set; }

		[Required(ErrorMessage = "The Client secret is required.")]
		public string? ValidAggregatorClientSecretHash {
			get { return validAggregatorClientSecretHash; }
			set { validAggregatorClientSecretHash = value; }
		}

		[Required(ErrorMessage = "The Discovery is required.")]
		public int? ValidDiscoveryServiceId { get; set; }

		[Required(ErrorMessage = "The Discovery Client ID is required.")]
		public string? ValidClientIdOnDiscovery { get; set; }

		[Required(ErrorMessage = "The Discovery Client secret is required.")]
		public string? ValidClientSecretOnDiscovery { get; set; }

		[Required(ErrorMessage = "The Discovery Redirect Uri is required.")]
		[Url(ErrorMessage = "Discovery Redirect Uri is not a valid fully-qualified http, https, or ftp URL.")]
		public string? ValidRedirectUriOnDiscovery { get; set; }

		[Url(ErrorMessage = "Redirect Uri is not a valid fully-qualified http, https, or ftp URL.")]
		[Required(ErrorMessage = "One or more Redirect Uri is required.")]
		public string? ValidSPRedirectUriFirst { get; set; }

		public string? ValidSPRedirectUriAnother { get; set; }

		[Url(ErrorMessage = "Notification Uri is not a valid fully-qualified http, https, or ftp URL.")]
		[Required(ErrorMessage = "One or more Notification Uri is required.")]
		public string? ValidSPNotificationUriFirst { get; set; }

		public string? ValidSPNotificationUriAnother { get; set; }

		[Required(ErrorMessage = "Billing сtn is required. Phone number without 7.")]
		[Range(9000000000, 9999999999, ErrorMessage = "Billing Ctn must be between 9000000000 and 9999999999")]
		public long ValidBillingCtn { get; set; }

		[Range(typeof(bool), "true", "true", ErrorMessage = "Jwks Uri or Jwks Value must be selected")]
		public bool ValidJwksUriOrValueSetted { get; set; }

		[Range(typeof(bool), "true", "true", ErrorMessage = "Client secretes are empty or unequal")]
		public bool ValidAggregatorClientSecretsAreEquals { get; set; }

		public bool FormIsClientSecretChanged { get; set; }

		public string? FormClientSecret1 { get; set; }

		public string? FormClientSecret2 { get; set; }

		[IPList(ErrorMessage = "Invalid IP list format")]
		public string? AllowedIPAddresses { get; set; }
	}
}