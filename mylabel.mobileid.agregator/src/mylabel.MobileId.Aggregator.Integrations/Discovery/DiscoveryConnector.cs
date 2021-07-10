using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Options;
using mylabel.MobileId.Aggregator.Integrations.Discovery.Model;
using Microsoft.Extensions.Logging;

namespace mylabel.MobileId.Aggregator.Integrations.Discovery
{
    public class DiscoveryConnector
    {
		ILogger<DiscoveryConnector> logger;
		HttpClient client;
        string redirectUri;

		const string dcidName = "dcid";
		const string mccParameterName = "Selected-MCC";
		const string mncParameterName = "Selected-MNC";
		const string redirectUriParameterName = "Redirect_URL";
		const string openIdConfigRel = "openid-configuration";
		const string premiumInfoRel = "userinfo";

		public DiscoveryConnector(HttpClient client, ILogger<DiscoveryConnector> logger)
        {
            this.client = client;
			this.logger = logger;
		}

        public void SetDiscoveryConnector(DiscoveryClientDetails s) 
        {
            client.BaseAddress = new Uri(s.Uri); 
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", CreateAuthHeader(s.ClientIdOnDiscovery, s.ClientSecretOnDiscovery));
			redirectUri = s.RedirectUri;
		}

        public async Task<DiscoSessionResponse> InitDiscoSessionRequestAsync()
        {
			var methodUri = $"?{redirectUriParameterName}={redirectUri}";
			var obj = new
			{
				uri = client.BaseAddress,
				query = methodUri,
				headers = client.DefaultRequestHeaders,
			};
			logger.LogInformation($"Discovery request: {JsonSerializer.Serialize(obj)}");

			var discoResponse = await client.GetAsync(methodUri);

			var discoResponseBody = await discoResponse.Content.ReadAsStringAsync();
			var obj2 = new
			{
				status = discoResponse.StatusCode,
				headers = discoResponse.Headers,
				body = discoResponseBody,
			};
			logger.LogInformation($"Discovery response: {JsonSerializer.Serialize(obj2)}");

			if (discoResponse.StatusCode == HttpStatusCode.Found)
            {
                var discoRedirectUri = discoResponse.Headers.Location;
                var dcid = HttpUtility.ParseQueryString(discoRedirectUri.Query).Get(dcidName);

				return new DiscoSessionResponse()
                {
                    Dcid = dcid,
                    CreatedByDiscoveryRedirectUri = discoRedirectUri.ToString(),
                };
            }
            else
                throw new Exception($"{discoResponse.StatusCode}. {discoResponse.Content.ToString()}");
        }

        public async Task<DiscoveryFullResponse> GetDiscoClientByMccMncAsync(string mcc_mnc)
        {
            var mcc = mcc_mnc.Substring(0, mcc_mnc.IndexOf("_"));
            var mnc = mcc_mnc.Substring(mcc_mnc.IndexOf("_") + 1);
            var requestUri = $"?{redirectUriParameterName}={redirectUri}&{mccParameterName}={mcc}&{mncParameterName}={mnc}";

			var obj = new
			{
				uri = client.BaseAddress,
				query = requestUri,
				headers = client.DefaultRequestHeaders,
			};
			logger.LogInformation($"Discovery request: {JsonSerializer.Serialize(obj)}");

			var response = await client.GetAsync(requestUri);
            var discoResponseString = await response.Content.ReadAsStringAsync();

			var obj2 = new
			{
				status = response.StatusCode,
				headers = response.Headers,
				body = discoResponseString,
			};
			logger.LogInformation($"Discovery response: {JsonSerializer.Serialize(obj2)}");

			return JsonSerializer.Deserialize<DiscoveryFullResponse>(discoResponseString!)!;	
		}

		public async Task<DiscoveryResponse> GetDiscoveryResponseByMsisdnAsync(DiscoveryRequest requestBody)
		{
			var body = JsonSerializer.Serialize(requestBody);
			var content = new StringContent(body);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var obj = new
			{
				uri = client.BaseAddress,
				query = string.Empty,
				headers = client.DefaultRequestHeaders,
				body = body,
			};
			logger.LogInformation($"Discovery request: {JsonSerializer.Serialize(obj)}");

			HttpResponseMessage response = await client.PostAsync(string.Empty, content);

			var discoResponseString = await response.Content.ReadAsStringAsync();
			var obj2 = new
			{
				status = response.StatusCode,
				headers = response.Headers,
				body = discoResponseString,
			};
			logger.LogInformation($"Discovery response: {JsonSerializer.Serialize(obj2)}");

			if (response.StatusCode == HttpStatusCode.OK)
			{				
				var fullResponse = JsonSerializer.Deserialize<DiscoveryFullResponse>(discoResponseString);
				var uriOpenIdConfig = fullResponse.Response.APIs.OperatorApiInfo.Link.Where(l => l.Rel == openIdConfigRel).FirstOrDefault()?.Uri!;
				var uriPremiumInfo = fullResponse.Response.APIs.OperatorApiInfo.Link.Where(l => l.Rel == premiumInfoRel).FirstOrDefault()?.Uri;
				return new DiscoveryResponse(fullResponse.Response.ClientId, fullResponse.Response.ClientSecret, fullResponse.Response.ServingOperator, uriOpenIdConfig, uriPremiumInfo);
			}
			else
			{
				throw new Exception();
			}
		}

		private string CreateAuthHeader(string clientId, string clientSecret)
		{
			return Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
		}
	}
}