using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Cryptography;
using mylabel.MobileId.Aggregator.Db;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using static System.Collections.Immutable.ImmutableDictionary;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.Extensions.Logging;

namespace mylabel.MobileId.Aggregator.Integrations.Idgw
{
	public class IdgwConnector
	{
		Settings settings;
		public MobileConnectConfiguration OpenIdConfig;
		public HttpClient HttpClient { get; }
		ILogger<IdgwConnector> logger;

		public IdgwConnector(
			HttpClient httpClient,
			IOptions<Settings> settings,
			MobileConnectConfiguration openIdConfig,
			ILogger<IdgwConnector> logger)
		{
			this.settings = settings.Value;
			OpenIdConfig = openIdConfig;
			HttpClient = httpClient;
			this.logger = logger;
		}

		public async Task<SIAuthorizationResult> SIAuthorizeAsync(string clientIdOnAggregator, string clientIdOnIdgw, string request)
		{
			var oldClaims = new JsonWebToken(request).Claims.ToImmutableDictionary(c => c.Type, c => c.Value);

			var aggregatorNotificationToken = Guid.NewGuid().ToString();
			var aggregatorNonce = Guid.NewGuid();

			Dictionary<string, object> newClaims = new()
			{
				[JwtRegisteredClaimNames.Aud] = string.Empty,
				[JwtRegisteredClaimNames.Iss] = settings.TokenIssuer!,
				[JwtRegisteredClaimNames.Nonce] = aggregatorNonce,
				[OpenIdConnectParameterNames.AcrValues] = oldClaims[OpenIdConnectParameterNames.AcrValues],
				[OpenIdConnectParameterNames.ClientId] = clientIdOnIdgw,
				[OpenIdConnectParameterNames.LoginHint] = oldClaims[OpenIdConnectParameterNames.LoginHint],
				[OpenIdConnectParameterNames.ResponseType] = MobileConnectResponseTypes.SIAsyncCode,
				[OpenIdConnectParameterNames.Scope] = oldClaims[OpenIdConnectParameterNames.Scope],
				[MobileConnectParameterNames.ClientNotificationToken] = aggregatorNotificationToken,
				[MobileConnectParameterNames.NotificationUri] = settings.NotificationUri!,
				[MobileConnectParameterNames.Version] = MobileConnectVersions.SIR2V10
			};

			var newRequest = SignedJwtCreator.Create(newClaims, settings.PrivateKey!, settings.Kid);

			var requestUri = OpenIdConfig.SIAuthorizationEndpoint
				+ $"?{OpenIdConnectParameterNames.ClientId}={clientIdOnIdgw}"
				+ $"&{OpenIdConnectParameterNames.ResponseType}={MobileConnectResponseTypes.SIAsyncCode}"
				+ $"&{OpenIdConnectParameterNames.Scope}={oldClaims[OpenIdConnectParameterNames.Scope]}"
				+ $"&{MobileConnectParameterNames.Request}={newRequest}";
			#region logger
			var obj = new
			{
				uri = HttpClient.BaseAddress,
				query = requestUri,
				headers = HttpClient.DefaultRequestHeaders,
			};
			logger.LogInformation($"Idgw request: {JsonSerializer.Serialize(obj)}");
			#endregion

			var idgwResponse = await HttpClient.GetAsync(requestUri);

			var responseBody = await idgwResponse.Content.ReadAsStringAsync();
			#region logger
			var obj2 = new
			{
				status = idgwResponse.StatusCode,
				headers = idgwResponse.Headers,
				body = responseBody,
			};
			logger.LogInformation($"Idgw response: {JsonSerializer.Serialize(obj2)}");
			#endregion

			if (!idgwResponse.IsSuccessStatusCode)
			{
				var unsuccess = JsonSerializer.Deserialize<IdgwUnsuccess>(responseBody)!;

				throw new UnifiedException(OAuth2ErrorDetails.GetError(unsuccess.Error), unsuccess.ErrorDescription);
			}

			var result = JsonSerializer.Deserialize<SIAuthorizationResult>(responseBody)!;
			result.SIAuthorizeRequest = new SIAuthorizationRequest()
			{
				SPNotificationUri = oldClaims[MobileConnectParameterNames.NotificationUri],
				SPNotificationToken = oldClaims[MobileConnectParameterNames.ClientNotificationToken],
				SPNonce = oldClaims.ContainsKey(JwtRegisteredClaimNames.Nonce) ? oldClaims[JwtRegisteredClaimNames.Nonce] : null,
				ClientIdOnAggregator = clientIdOnAggregator,
				IdgwJwksUri = OpenIdConfig.JwksUri,
				AuthorizationRequestId = result.AuthorizationRequestId,
				AggregatorNotificationToken = aggregatorNotificationToken,
				AggregatorNonce = aggregatorNonce,
				CreatedAt = DateTimeOffset.Now,
				PremiumInfoEndpoint = OpenIdConfig.PremiumInfoEndpoint,
			};
			return result;
		}

		public async Task<string> PremiumInfoAsync(string token, PremiumInfoBody? requestBody)
		{
			HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			var message = requestBody != null ?
				new HttpRequestMessage(HttpMethod.Post, OpenIdConfig.PremiumInfoEndpoint!) { Content = new StringContent(JsonSerializer.Serialize(requestBody)) } :
				new HttpRequestMessage(HttpMethod.Post, OpenIdConfig.PremiumInfoEndpoint!);
			#region logger
			var obj = new
			{
				uri = HttpClient.BaseAddress,
				query = message.RequestUri,
				headers = HttpClient.DefaultRequestHeaders,
			};
			logger.LogInformation($"Idgw request: {JsonSerializer.Serialize(obj)}");
			#endregion

			HttpResponseMessage premiumInfoResponse = await HttpClient.SendAsync(message!);

			if (premiumInfoResponse.StatusCode == HttpStatusCode.MethodNotAllowed)
			{
				message = new HttpRequestMessage(HttpMethod.Get, OpenIdConfig.PremiumInfoEndpoint!);
				premiumInfoResponse = await HttpClient.SendAsync(message!);
			}

			if (!premiumInfoResponse.IsSuccessStatusCode)
			{
				var unsuccess = await premiumInfoResponse.Content.ReadFromJsonAsync<IdgwUnsuccess>();
				#region logger
				var obj2 = new
				{
					status = premiumInfoResponse.StatusCode,
					headers = premiumInfoResponse.Headers,
					body = unsuccess,
				};
				logger.LogInformation($"Idgw response: {JsonSerializer.Serialize(obj2)}");
				#endregion

				if (premiumInfoResponse.StatusCode == HttpStatusCode.Unauthorized) //FIX, Ресурсный присылает 401 с непонятным Error
				{
					throw new UnifiedException(OAuth2Error.UnauthorizedClient, unsuccess.ErrorDescription);
				}
				throw new UnifiedException(OAuth2ErrorDetails.GetError(unsuccess!.Error), unsuccess.ErrorDescription);
			}
			var premiumInfoResponseBody = await premiumInfoResponse.Content.ReadAsStringAsync();
			#region logger
			var obj3 = new
			{
				status = premiumInfoResponse.StatusCode,
				headers = premiumInfoResponse.Headers,
				body = premiumInfoResponseBody,
			};
			logger.LogInformation($"Idgw response: {JsonSerializer.Serialize(obj3)}");
			#endregion

			return premiumInfoResponseBody;
		}

		public Task<string> GetJwksAsync() => HttpClient.GetStringAsync(OpenIdConfig.JwksUri);

		public class SIAuthorizationResult
		{
			public SIAuthorizationRequest? SIAuthorizeRequest { get; set; }
			[property: JsonPropertyName(MobileConnectParameterNames.AuthorizationRequestId)] public Guid AuthorizationRequestId { get; set; }
			[property: JsonPropertyName(OpenIdConnectParameterNames.ExpiresIn)] public int ExpiresIn { get; set; }
		}
	}

	public class PremiumInfoBody
	{
		[JsonPropertyName("mc_claims")] public Dictionary<string, string>? Claims { get; set; }
	}
}