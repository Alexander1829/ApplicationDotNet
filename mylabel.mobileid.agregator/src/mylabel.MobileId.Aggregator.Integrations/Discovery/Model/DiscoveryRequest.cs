using System.Text.Json.Serialization;

namespace mylabel.MobileId.Aggregator.Integrations.Discovery.Model
{
    public class DiscoveryRequest
    {
        public DiscoveryRequest(string msisdn, string redirectUri)
        {
            Msisdn = msisdn;
            RedirectUri = redirectUri;
        }
        [JsonPropertyName("MSISDN")]
        public string Msisdn { get; set; }

        [JsonPropertyName("Redirect_URL")]
        public string RedirectUri { get; set; }        
    }
}