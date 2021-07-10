using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mylabel.MobileId.Aggregator.Integrations.Discovery.Model
{
    public class DiscoveryResponse
    {
        public DiscoveryResponse(string clientId, string clientSecret, string servingOperator, string uriOpenIdConfig, string? uriPremiumInfo) 
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
			ServingOperator = servingOperator;
			UriOpenIdConfig = uriOpenIdConfig;
			UriPremiumInfo = uriPremiumInfo;
		}
        public string ClientId { get; set; }
        public string ClientSecret { get;set;}
		public string ServingOperator { get; set; }
		public string UriOpenIdConfig { get; set; }
		public string? UriPremiumInfo { get; set; }
		public string SubscriberId { get; set; }
    }
}
