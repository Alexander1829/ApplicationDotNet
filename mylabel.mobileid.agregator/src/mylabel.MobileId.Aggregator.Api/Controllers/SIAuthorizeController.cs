using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.BusinessLogic;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Db;
using mylabel.MobileId.Aggregator.Integrations.Discovery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace mylabel.MobileId.Aggregator.Api.Controllers
{
	[ApiController]
	public class SIAuthorizeController : ControllerBase
	{
		SIRequestValidationService siRequestValidationService;
		SIRequestAuthorizationService siRequestAuthorizationService;
		IdgwConnectorManager idgwConnectorManager;
		DiscoveryConnector discoveryConnector;

		public SIAuthorizeController(
			SIRequestValidationService siRequestValidationService,
			SIRequestAuthorizationService siRequestAuthorizationService,
			IdgwConnectorManager idgwConnectorManager,
			DiscoveryConnector discoveryConnector)
		{
			this.siRequestValidationService = siRequestValidationService;
			this.siRequestAuthorizationService = siRequestAuthorizationService;
			this.idgwConnectorManager = idgwConnectorManager;
			this.discoveryConnector = discoveryConnector;
		}

		public record SIAuthorizationResult(
			[property: JsonPropertyName(MobileConnectParameterNames.AuthorizationRequestId)] Guid AuthorizationRequestId,
			[property: JsonPropertyName(OpenIdConnectParameterNames.ExpiresIn)] int ExpiresIn);
		[HttpGet("si-authorize")]
		public async Task<SIAuthorizationResult> SIAuthorize([FromQuery(Name = OpenIdConnectParameterNames.ClientId)] string? clientId, string? scope, string? request)
		{
			var jwtPayload = siRequestValidationService.ValidateAndGetJwtPayload(clientId, scope, request);

			var discoDetails = await siRequestAuthorizationService.AuthorizeAndGetDiscoDetailsAsync(clientId!, request!);

			discoveryConnector.SetDiscoveryConnector(new(discoDetails.DiscoUri, discoDetails.DiscoClientId, discoDetails.DiscoSecret, discoDetails.DiscoRedirectUri));

			var discoveryResponse = await discoveryConnector.GetDiscoveryResponseByMsisdnAsync(new(jwtPayload.Msisdn, discoDetails.DiscoRedirectUri));

			var isNewIdgwConnector = await idgwConnectorManager.TryAddConnectorAsync(discoveryResponse.ServingOperator!, discoveryResponse.UriOpenIdConfig);

			var servingOperator = idgwConnectorManager.GetServingOperatorByString(discoveryResponse.ServingOperator!);

			var idgwConnector = idgwConnectorManager[servingOperator];

			var idgwSIAuthorizeResult = await idgwConnector.SIAuthorizeAsync(clientId!, discoveryResponse.ClientId, request!);
			idgwSIAuthorizeResult.SIAuthorizeRequest!.ServingOperator = discoveryResponse.ServingOperator!;
			idgwSIAuthorizeResult.SIAuthorizeRequest.Msisdn = jwtPayload.Msisdn;
			idgwSIAuthorizeResult.SIAuthorizeRequest.AcrValues = jwtPayload.AcrValues;
			idgwSIAuthorizeResult.SIAuthorizeRequest.Scope = scope;

			await AggregatorContext.SaveAsync(ctx => ctx.SIAuthorizationRequests!.Add(idgwSIAuthorizeResult.SIAuthorizeRequest!));

			return new(idgwSIAuthorizeResult.AuthorizationRequestId, idgwSIAuthorizeResult.ExpiresIn);
		}
	}
}