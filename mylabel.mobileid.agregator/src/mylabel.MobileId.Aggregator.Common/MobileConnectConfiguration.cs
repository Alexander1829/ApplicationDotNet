using System.Text.Json;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace mylabel.MobileId.Aggregator.Common
{
	public class MobileConnectConfiguration : OpenIdConnectConfiguration
	{
		public MobileConnectConfiguration(string json) : base(json)
		{
			var jsonRoot = JsonDocument.Parse(json).RootElement;
			SIAuthorizationEndpoint = jsonRoot.TryGetProperty("si_authorization_endpoint", out var siAuthorizationEndpoint) ? siAuthorizationEndpoint.GetString() : null;
			PremiumInfoEndpoint = jsonRoot.TryGetProperty("premiuminfo_endpoint", out var premiumInfoEndpoint) ? premiumInfoEndpoint.GetString() : null;
		}

		public string? SIAuthorizationEndpoint { get; }
		public string? PremiumInfoEndpoint { get; set; }
	}
}