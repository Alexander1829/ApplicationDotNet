using System.Text.Json.Serialization;

namespace mylabel.MobileId.Aggregator.Integrations.Discovery.Model
{
    public class DiscoveryFullResponse
    {
        [JsonPropertyName("ttl")]
        public int TTL { get; set; }

        [JsonPropertyName("subscriber_id")]
        public string SubscriberId { get; set; }

        [JsonPropertyName("response")]
        public DiscoveryResponseResponse Response { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("description")]
        public string ErrorDescription { get; set; }
    }

    public class DiscoveryResponseResponse
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("client_name")]
        public string ClientName { get; set; }

        [JsonPropertyName("serving_operator")]
        public string ServingOperator { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("apis")]
        public DiscoveryResponseApiInfo APIs { get; set; }
    }

    public class DiscoveryResponseApiInfo
    {
        [JsonPropertyName("operatorid")]
        public DiscoveryResponseOperatorApiInfo OperatorApiInfo { get; set; }
    }

    public class DiscoveryResponseOperatorApiInfo
    {
        [JsonPropertyName("link")]
        public DiscoveryResponseLink[] Link { get; set; }
    }

    public class DiscoveryResponseLink
    {
        [JsonPropertyName("href")]
        public string Uri { get; set; }

        [JsonPropertyName("rel")]
        public string Rel { get; set; }
    }
}
