namespace mylabel.MobileId.Aggregator.Integrations.Discovery.Model
{
    public class DiscoveryClientDetails
    {
		public DiscoveryClientDetails(string uri, string clientIdOnDiscovery, string clientSecretOnDiscovery, string redirectUri)
		{
			Uri = uri; ClientIdOnDiscovery = clientIdOnDiscovery; ClientSecretOnDiscovery = clientSecretOnDiscovery; RedirectUri = redirectUri;
		}
		public string Uri { get; set; }
		public string ClientIdOnDiscovery { get; set; }
        public string ClientSecretOnDiscovery { get; set; }
        public string RedirectUri { get; set; }
	}
}
