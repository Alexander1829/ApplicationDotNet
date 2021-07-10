using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.BusinessLogic;
using mylabel.MobileId.Aggregator.BusinessLogic.MobileConnect;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Cryptography;
using mylabel.MobileId.Aggregator.Db;
using mylabel.MobileId.Aggregator.Integrations.Idgw;
using mylabel.MobileId.Aggregator.Jobs.JobList.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace mylabel.MobileId.Aggregator.Api.Controllers
{
	[ApiController]
	public class MCTokenController : ControllerBase
	{
		IdgwConnectorManager idgwConnectorManager;
		Settings settings;
		StatePremiumInfoService statePremiumInfo;
		DIMCTokenService tokenService;
		ILogger<MCTokenController> logger;

		const string Basic = "Basic";

		public MCTokenController(
			IdgwConnectorManager idgwConnectorManager,
			IOptions<Settings> settings,
			StatePremiumInfoService statePremiumInfo,
			DIMCTokenService tokenService,
			ILogger<MCTokenController> logger
			)
		{
			this.settings = settings.Value;
			this.idgwConnectorManager = idgwConnectorManager;
			this.statePremiumInfo = statePremiumInfo;
			this.tokenService = tokenService;
			this.logger = logger;
		}

		[HttpPost("mc-token")]
		public async Task<MCTokenResult> MCToken(
			[FromForm(Name = OpenIdConnectParameterNames.Code)] string code,
			[FromForm(Name = OpenIdConnectParameterNames.RedirectUri)] string redirectUri,
			[FromForm(Name = OpenIdConnectParameterNames.State)] string state,
			[FromForm(Name = OpenIdConnectParameterNames.GrantType)] string grantType
			)
		{
			await tokenService.EnsureIncomingCredentialsAreValidAsync(Request.Headers[HeaderNames.Authorization]);

			var authorizationRequest = await AggregatorContext.QueryAsync(ctx => ctx.DIAuthorizationRequests!.FirstAsync(l => l.Code == code));

			StringContent content = new StringContent($"{OpenIdConnectParameterNames.Code}={code}"
				+ $"&{OpenIdConnectParameterNames.RedirectUri}={settings.DIAuthorizeIdgwCallbackUri}"
				+ $"&{OpenIdConnectParameterNames.State}={state}"
				+ $"&{OpenIdConnectParameterNames.GrantType}={grantType}");
			content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

			var servingOperator = (ServingOperator)Enum.Parse(typeof(ServingOperator), authorizationRequest!.ServingOperator!, true);

			var idgwConnector = idgwConnectorManager[servingOperator];

			using var message = new HttpRequestMessage(HttpMethod.Post, idgwConnector!.OpenIdConfig.TokenEndpoint) { Content = content };

			var basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authorizationRequest.IdgwClientId}:{authorizationRequest.IdgwClientSecret}"));

			message.Headers.Add(HeaderNames.Authorization, $"{Basic} {basicAuthValue}");
			#region logger
			var obj = new
			{
				uri = idgwConnector.HttpClient.BaseAddress,
				query = message.RequestUri,
				headers = idgwConnector.HttpClient.DefaultRequestHeaders,
			};
			logger.LogInformation($"Idgw request: {JsonSerializer.Serialize(obj)}");
			#endregion

			var idgwResponse = await idgwConnector.HttpClient.SendAsync(message);

			if (!idgwResponse.IsSuccessStatusCode)
			{
				var unsuccess = await idgwResponse.Content.ReadFromJsonAsync<IdgwUnsuccess>();
				#region logger
				var obj2 = new
				{
					status = idgwResponse.StatusCode,
					headers = idgwResponse.Headers,
					body = unsuccess,
				};
				logger.LogInformation($"Idgw response: {JsonSerializer.Serialize(obj2)}");
				#endregion

				throw new UnifiedException(OAuth2ErrorDetails.GetError(unsuccess!.Error), unsuccess.ErrorDescription);
			}

			var idgwResponseBody = await idgwResponse.Content.ReadFromJsonAsync<MCTokenResult>();
			#region logger
			var obj3 = new
			{
				status = idgwResponse.StatusCode,
				headers = idgwResponse.Headers,
				body = idgwResponseBody,
			};
			logger.LogInformation($"Idgw response: {JsonSerializer.Serialize(obj3)}");
			#endregion

			JwtAccessToken accessToken = new(idgwResponseBody!.AccessToken);

			var idClaims = new JsonWebToken(idgwResponseBody.IdToken).Claims.ToImmutableDictionary(c => c.Type, c => c.Value);

			accessToken.Aud = string.Empty;
			accessToken.Azp = Guid.NewGuid().ToString();//
			accessToken.Exp ??= long.Parse(idClaims[JwtRegisteredClaimNames.Exp]);
			accessToken.Iat = long.Parse(idClaims[JwtRegisteredClaimNames.Iat]);
			accessToken.Iss = settings.TokenIssuer!;
			accessToken.Jti = Guid.NewGuid();
			accessToken.Sub = idClaims![JwtRegisteredClaimNames.Sub];

			var newAccessToken = accessToken.SignCurrentState(settings.PrivateKey!, settings.Kid);

			Dictionary<string, object> newIdClaims = new()
			{
				[JwtRegisteredClaimNames.AtHash] = newAccessToken.AtHash,
				[JwtRegisteredClaimNames.Sub] = idClaims[JwtRegisteredClaimNames.Sub],
				[JwtRegisteredClaimNames.Amr] = new[] { idClaims[JwtRegisteredClaimNames.Amr] },
				[JsonWebKeyParameterNames.Kid] = idClaims[JsonWebKeyParameterNames.Kid],
				[JwtRegisteredClaimNames.Iss] = settings.TokenIssuer!,
				[JwtRegisteredClaimNames.Nonce] = idClaims[JwtRegisteredClaimNames.Nonce],
				[JwtRegisteredClaimNames.Aud] = Guid.NewGuid(),//
				[JwtRegisteredClaimNames.Acr] = int.Parse(idClaims[JwtRegisteredClaimNames.Acr]),
				[JwtRegisteredClaimNames.Azp] = Guid.NewGuid(),//
				[JwtRegisteredClaimNames.AuthTime] = long.Parse(idClaims[JwtRegisteredClaimNames.AuthTime]),
				[JwtRegisteredClaimNames.Exp] = long.Parse(idClaims[JwtRegisteredClaimNames.Exp]),
				[JwtRegisteredClaimNames.Iat] = long.Parse(idClaims[JwtRegisteredClaimNames.Iat]),
				[JwtRegisteredClaimNames.Jti] = Guid.NewGuid()
			};

			var newIdToken = SignedJwtCreator.Create(newIdClaims, settings.PrivateKey!, settings.Kid);

			PremiumInfoToken premiumInfoToken = new PremiumInfoToken()
			{
				AccessTokenOnAggregatorHash = newAccessToken.AtHash,
				AccessTokenOnIdgw = idgwResponseBody!.AccessToken,
				CreatedAt = DateTimeOffset.Now,
				AfterSiOrDi = "DI",
			};

			authorizationRequest.AccessTokenOnAggregatorHash = newAccessToken.AtHash;
			authorizationRequest.IdgwClientSecret = null;

			var serviceProvider = await AggregatorContext.QueryAsync(ctx => ctx.ServiceProviders!.FirstAsync(l => l.ClientIdOnAggregator == authorizationRequest!.ClientId));

			var billingTransaction = BillingTransactionFactory.CreateNew(
				serviceProvider.BillingCtn!,
				authorizationRequest.Msisdn,
				authorizationRequest.Msisdn,
				authorizationRequest.Scope!,
				authorizationRequest.ServingOperator!,
				authorizationRequest.AcrValues!,
				BillingFrom.Authorization,
				authorizationRequest.Id,
				"DI");
			await AggregatorContext.SaveAsync(ctx =>
			{
				ctx.PremiumInfoTokens!.AddAsync(premiumInfoToken);
				ctx.DIAuthorizationRequests!.Update(authorizationRequest);
			});
			if (billingTransaction != null)
				await AggregatorContext.SaveAsync(ctx => ctx.BillingTransactions!.AddAsync(billingTransaction));

			return idgwResponseBody with
			{
				AccessToken = newAccessToken.Jwt,
				IdToken = newIdToken
			};
		}

		public record MCTokenResult(
			[property: JsonPropertyName(OpenIdConnectParameterNames.AccessToken)] string AccessToken,
			[property: JsonPropertyName(OpenIdConnectParameterNames.TokenType)] string TokenType,
			[property: JsonPropertyName(OpenIdConnectParameterNames.ExpiresIn)] int ExpiresIn,
			[property: JsonPropertyName(OpenIdConnectParameterNames.Scope)] string Scope,
			[property: JsonPropertyName(OpenIdConnectParameterNames.IdToken)] string IdToken);
	}
}