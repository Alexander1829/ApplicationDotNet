using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.BusinessLogic;
using mylabel.MobileId.Aggregator.BusinessLogic.MobileConnect;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Cryptography;
using mylabel.MobileId.Aggregator.Db;
using mylabel.MobileId.Aggregator.Jobs.JobList.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace mylabel.MobileId.Aggregator.Api.Controllers
{
	[ApiController]
	public class NotificationController : ControllerBase
	{
		const string Bearer = "Bearer";

		IdgwConnectorManager idgwConnectorManager;
		HttpClient httpClient;
		Settings settings;

		public NotificationController(
			HttpClient httpClient,
			IOptions<Settings> settings,
			IdgwConnectorManager idgwConnectorManager)
		{
			this.httpClient = httpClient;
			this.settings = settings.Value;
			this.idgwConnectorManager = idgwConnectorManager;
		}

		[HttpPost("notification")]
		public async Task<NoContentResult> Notification([FromBody] NotificationRequest notificationRequest)
		{
			var authorizationRequest = await AggregatorContext.QueryAsync(ctx => ctx.SIAuthorizationRequests!.FirstAsync(l => l.AuthorizationRequestId == notificationRequest.AuthorizationRequestId!));

			if (!Request.Headers[HeaderNames.Authorization].Contains($"{Bearer} {authorizationRequest.AggregatorNotificationToken}"))
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, "No valid token found");

			if (notificationRequest.Error != null)
			{
				Dictionary<string, object> content = new()
				{
					[MobileConnectParameterNames.AuthorizationRequestId] = notificationRequest.AuthorizationRequestId,
					[OpenIdConnectParameterNames.Error] = notificationRequest.Error
				};
				if (notificationRequest.ErrorDescription != null)
					content.Add(OpenIdConnectParameterNames.ErrorDescription, notificationRequest.ErrorDescription);
				await SendToSPAndSaveAsync(authorizationRequest, content);
				return NoContent();
			}

			if (new JsonWebToken(notificationRequest.IdToken).GetPayloadValue<Guid>(JwtRegisteredClaimNames.Nonce) != authorizationRequest.AggregatorNonce)
				throw new UnifiedException(OAuth2Error.InvalidRequest, "Invalid nonce");

			try
			{
				var servingOperator = idgwConnectorManager.GetServingOperatorByString(authorizationRequest.ServingOperator!);
				var idgwConnector = idgwConnectorManager[servingOperator];
				var idgwJwks = await idgwConnector.GetJwksAsync();

				JwtAccessToken accessToken = new(notificationRequest.AccessToken!, idgwJwks);
				if (accessToken.SourceSignatureIsValid == false)
					throw new UnifiedException(OAuth2Error.UnauthorizedClient);

				if (!JwtSignatureValidator.Validate(notificationRequest.IdToken!, idgwJwks, out var idClaims))
					throw new UnifiedException(OAuth2Error.UnauthorizedClient);

				accessToken.Aud = string.Empty;
				accessToken.Azp = authorizationRequest.ClientIdOnAggregator!;
				accessToken.Exp ??= long.Parse(idClaims![JwtRegisteredClaimNames.Exp].ToString()!);
				accessToken.Iat = long.Parse(idClaims![JwtRegisteredClaimNames.Iat].ToString()!);
				accessToken.Iss = settings.TokenIssuer!;
				accessToken.Jti = Guid.NewGuid();
				accessToken.Sub = (string)idClaims![JwtRegisteredClaimNames.Sub];

				var newAccessToken = accessToken.SignCurrentState(settings.PrivateKey!, settings.Kid);

				Dictionary<string, object> newIdClaims = new()
				{
					[JwtRegisteredClaimNames.AtHash] = newAccessToken.AtHash,
					[JwtRegisteredClaimNames.Sub] = idClaims[JwtRegisteredClaimNames.Sub],
					[JwtRegisteredClaimNames.Amr] = idClaims[JwtRegisteredClaimNames.Amr],
					[JsonWebKeyParameterNames.Kid] = idClaims[JsonWebKeyParameterNames.Kid],
					[JwtRegisteredClaimNames.Iss] = settings.TokenIssuer!,
					[JwtRegisteredClaimNames.Aud] = authorizationRequest.ClientIdOnAggregator!,
					[JwtRegisteredClaimNames.Acr] = idClaims[JwtRegisteredClaimNames.Acr],
					[JwtRegisteredClaimNames.Azp] = authorizationRequest.ClientIdOnAggregator!,
					[JwtRegisteredClaimNames.AuthTime] = idClaims[JwtRegisteredClaimNames.AuthTime],
					[MobileConnectParameterNames.Recipient] = idClaims[MobileConnectParameterNames.Recipient],
					[JwtRegisteredClaimNames.Iat] = idClaims[JwtRegisteredClaimNames.Iat],
					[JwtRegisteredClaimNames.Exp] = idClaims[JwtRegisteredClaimNames.Exp],
					[JwtRegisteredClaimNames.Jti] = Guid.NewGuid()
				};
				if (authorizationRequest.SPNonce != null)
					newIdClaims.Add(JwtRegisteredClaimNames.Nonce, authorizationRequest.SPNonce);

				var newIdToken = SignedJwtCreator.Create(newIdClaims, settings.PrivateKey!, settings.Kid);

				await SendToSPAndSaveAsync(authorizationRequest, new()
				{
					[OpenIdConnectParameterNames.AccessToken] = newAccessToken.Jwt,
					[MobileConnectParameterNames.AuthorizationRequestId] = notificationRequest.AuthorizationRequestId,
					[OpenIdConnectParameterNames.IdToken] = newIdToken,
					[OpenIdConnectParameterNames.TokenType] = notificationRequest.TokenType!,
					[OpenIdConnectParameterNames.ExpiresIn] = notificationRequest.ExpiresIn!.Value
				});
				var serviceProvider = await AggregatorContext.QueryAsync(ctx => ctx.ServiceProviders!.FirstAsync(l => l.ClientIdOnAggregator == authorizationRequest.ClientIdOnAggregator));

				var billingTransaction = BillingTransactionFactory.CreateNew(
					serviceProvider.BillingCtn!,
					authorizationRequest.Msisdn!,
					authorizationRequest.Msisdn!,
					authorizationRequest.Scope!,
					authorizationRequest.ServingOperator!,
					authorizationRequest.AcrValues!,
					BillingFrom.Authorization,
					authorizationRequest.Id,
					"SI");

				authorizationRequest.AccessTokenOnAggregatorHash = newAccessToken.AtHash;
				var premiumInfoToken = new PremiumInfoToken()
				{
					AccessTokenOnAggregatorHash = newAccessToken.AtHash,
					AccessTokenOnIdgw = notificationRequest.AccessToken,
					CreatedAt = DateTimeOffset.Now,
					AfterSiOrDi = "SI",
				};
				await AggregatorContext.SaveAsync(ctx =>
				{
					ctx.PremiumInfoTokens!.AddAsync(premiumInfoToken);
					ctx.SIAuthorizationRequests!.Update(authorizationRequest);					
				});
				if (billingTransaction != null)
					await AggregatorContext.SaveAsync(ctx => ctx.BillingTransactions!.AddAsync(billingTransaction));					
			}
			catch
			{
				Dictionary<string, object> content = new()
				{
					[MobileConnectParameterNames.AuthorizationRequestId] = notificationRequest.AuthorizationRequestId,
					[OpenIdConnectParameterNames.Error] = OAuth2ErrorDetails.GetText(OAuth2Error.ServerError)
				};
				await SendToSPAndSaveAsync(authorizationRequest, content);
				throw;
			}

			return NoContent();
		}

		async Task SendToSPAndSaveAsync(SIAuthorizationRequest authorizationRequest, Dictionary<string, object> content)
		{
			using var jsonContent = JsonContent.Create(content);
			using HttpRequestMessage httpRequestMessage = new(HttpMethod.Post, authorizationRequest.SPNotificationUri) { Content = jsonContent };
			httpRequestMessage.Headers.Authorization = new(Bearer, authorizationRequest.SPNotificationToken);
			var httpTask = httpClient.SendAsync(httpRequestMessage);

			authorizationRequest.RespondedAt = DateTimeOffset.Now;
			var dbTask = AggregatorContext.SaveAsync(ctx => ctx.SIAuthorizationRequests!.Update(authorizationRequest));

			await httpTask;
			await dbTask;
		}

		public record NotificationRequest(
			[property: JsonPropertyName(MobileConnectParameterNames.AuthorizationRequestId)] Guid AuthorizationRequestId,

			[property: JsonPropertyName(OpenIdConnectParameterNames.AccessToken)] string? AccessToken,
			[property: JsonPropertyName(OpenIdConnectParameterNames.IdToken)] string? IdToken,
			[property: JsonPropertyName(OpenIdConnectParameterNames.TokenType)] string? TokenType,
			[property: JsonPropertyName(OpenIdConnectParameterNames.ExpiresIn)] int? ExpiresIn,

			[property: JsonPropertyName(OpenIdConnectParameterNames.Error)] string? Error,
			[property: JsonPropertyName(OpenIdConnectParameterNames.ErrorDescription)] string? ErrorDescription
			);
	}
}