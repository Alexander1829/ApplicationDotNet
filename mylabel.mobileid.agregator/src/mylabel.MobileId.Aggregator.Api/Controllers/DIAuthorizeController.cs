using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.BusinessLogic;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Cryptography;
using mylabel.MobileId.Aggregator.Db;
using mylabel.MobileId.Aggregator.Integrations.Discovery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace mylabel.MobileId.Aggregator.Api.Controllers
{
	[ApiController]
	public class DIAuthorizeController : ControllerBase
	{
		DiscoveryConnector discoveryConnector;
		IdgwConnectorManager idgwConnectorManager;
		DIRequestValidationService diRequestValidationService;
		DIRequestAuthorizationService diAuthorizationService;
		Settings settings;

		public DIAuthorizeController(DiscoveryConnector discoveryConnector, IdgwConnectorManager idgwConnectorManager, DIRequestValidationService diRequestValidationService, DIRequestAuthorizationService diAuthorizationService, IOptions<Settings> settings)
		{
			this.discoveryConnector = discoveryConnector;
			this.idgwConnectorManager = idgwConnectorManager;
			this.diRequestValidationService = diRequestValidationService;
			this.diAuthorizationService = diAuthorizationService;
			this.settings = settings.Value;
		}

		[HttpGet("authorize")]
		public async Task<object> DIAuthorize(
			[FromQuery(Name = OpenIdConnectParameterNames.ClientId)] string? clientId,
			[FromQuery(Name = OpenIdConnectParameterNames.RedirectUri)] string? redirectUri,
			[FromQuery(Name = OpenIdConnectParameterNames.Scope)] string? scope,
			[FromQuery(Name = OpenIdConnectParameterNames.ResponseType)] string? responseType,
			[FromQuery(Name = OpenIdConnectParameterNames.AcrValues)] string? acrValues,
			[FromQuery(Name = OpenIdConnectParameterNames.Nonce)] string? nonce,
			[FromQuery(Name = MobileConnectParameterNames.Version)] string? version,
			[FromQuery(Name = OpenIdConnectParameterNames.State)] string? state,
			[FromQuery(Name = OpenIdConnectParameterNames.LoginHint)] string? loginHint,
			[FromQuery(Name = OpenIdConnectParameterNames.Display)] string? display,
			[FromQuery(Name = MobileConnectParameterNames.ClientName)] string? clientName)
		{
			var createdAt = DateTimeOffset.Now;
			var serviceProvider = await diRequestValidationService.ValidateWithoutRedirectAsync(clientId, redirectUri);
			try
			{
#if DEBUG
				nonce = Guid.NewGuid().ToString(); state = Guid.NewGuid().ToString();
#endif
				var diAuthorizationRequest = new DIAuthorizationRequest()
				{
					ClientId = clientId,
					RedirectUri = redirectUri,
					Scope = scope,
					ResponseType = responseType,
					AcrValues = acrValues,
					State = state,
					Nonce = nonce,
					Version = version,
					LoginHint = loginHint,
					Display = display,
					ClientName = clientName,
					CreatedAt = createdAt
				};
				diAuthorizationRequest.StateNew = Guid.NewGuid().ToString();

				diRequestValidationService.ValidateWithRedirect(diAuthorizationRequest);

				if(diAuthorizationRequest.LoginHint?.IndexOf("MSISDN:") == 0)
					diAuthorizationRequest.Msisdn = diAuthorizationRequest.LoginHint!.Substring("MSISDN:".Length);
				await AggregatorContext.SaveAsync(ctx => ctx.DIAuthorizationRequests!.AddAsync(diAuthorizationRequest));
				SetDICookieStateNew(diAuthorizationRequest.StateNew);

				var discoClientDetails = await diAuthorizationService.GetDiscoSettingsByClientIdAsync(clientId!);
				discoveryConnector.SetDiscoveryConnector(discoClientDetails);

				if (string.IsNullOrEmpty(loginHint))
				{
					var discoSessionResponse = await discoveryConnector.InitDiscoSessionRequestAsync(); // внутри Discovery хотел редирекнуть, но мы пока не редиректим.

					diAuthorizationRequest.Dcid = discoSessionResponse.Dcid!;
					await AggregatorContext.SaveAsync(ctx => ctx.DIAuthorizationRequests!.Update(diAuthorizationRequest));

					return Redirect(discoSessionResponse.CreatedByDiscoveryRedirectUri);
				}
				var discoveryResponse = await discoveryConnector.GetDiscoveryResponseByMsisdnAsync(new(loginHint.Substring(loginHint.IndexOf(":") + 1), discoClientDetails.RedirectUri));

				diAuthorizationRequest.ServingOperator = discoveryResponse.ServingOperator;
				diAuthorizationRequest.IdgwClientId = discoveryResponse.ClientId;
				diAuthorizationRequest.IdgwClientSecret = discoveryResponse.ClientSecret;
				await AggregatorContext.SaveAsync(ctx => ctx.DIAuthorizationRequests!.Update(diAuthorizationRequest));
				
				var isNewIdgwConnector = await idgwConnectorManager.TryAddConnectorAsync(discoveryResponse.ServingOperator!, discoveryResponse.UriOpenIdConfig);
				var servingOperator = idgwConnectorManager.GetServingOperatorByString(discoveryResponse.ServingOperator!);
				var idgwConnector = idgwConnectorManager[servingOperator];

				var idGatewayDIAuthUri = diAuthorizationService.CreateIdGatewayDIAuthUri(idgwConnector.OpenIdConfig.AuthorizationEndpoint, diAuthorizationRequest);

				return Redirect(idGatewayDIAuthUri);
			}
			catch (UnifiedException ex)
			{
				return Redirect($"{redirectUri}?error={ex.Error}&error_description={ex.ErrorDescription}");
			}
			catch (Exception ex)
			{
				return Redirect($"{redirectUri}?error=server_error");
			}
		}

		[HttpGet("api/v1/discoverycallback")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> DiscoveryCallBack(
			string? dcid,
			[FromQuery(Name = "mcc_mnc")] string? mccMnc,
			[FromQuery(Name = "subscriber_id")] string? encryptMsisdn,
			string? error,
			string? error_description)
		{
			string? cookiesStateNew = GetDICookieStateNew();
			DIAuthorizationRequest? diAuthorizationRequest = await GetDiAuthRequestByDcidAsync(cookiesStateNew, dcid);
			try
			{
				await diRequestValidationService.ValidateDiscoCallBackAsync(diAuthorizationRequest!, error, error_description);

				discoveryConnector.SetDiscoveryConnector(await diAuthorizationService.GetDiscoSettingsByClientIdAsync(diAuthorizationRequest!.ClientId!));

				var discoByMccMncResponse = await discoveryConnector.GetDiscoClientByMccMncAsync(mccMnc!);

				diAuthorizationRequest!.ServingOperator = discoByMccMncResponse.Response.ServingOperator;
				diAuthorizationRequest!.IdgwClientId = discoByMccMncResponse.Response.ClientId;
				diAuthorizationRequest!.IdgwClientSecret = discoByMccMncResponse.Response.ClientSecret;
				if (string.IsNullOrEmpty(diAuthorizationRequest!.ClientName))
					diAuthorizationRequest!.ClientName = discoByMccMncResponse.Response.ClientName;
				await AggregatorContext.SaveAsync(ctx => ctx.DIAuthorizationRequests!.Update(diAuthorizationRequest));
				diAuthorizationRequest!.LoginHint = "ENCR_MSISDN:" + encryptMsisdn!;

				var isNewIdgwConnector = await idgwConnectorManager.TryAddConnectorAsync(discoByMccMncResponse.Response.ServingOperator, diAuthorizationService.GetOpenidConfigUri(discoByMccMncResponse));
				var servingOperator = idgwConnectorManager.GetServingOperatorByString(discoByMccMncResponse.Response.ServingOperator);
				var idgwConnector = idgwConnectorManager[servingOperator];

				var idGatewayDIAuthUri = diAuthorizationService.CreateIdGatewayDIAuthUri(idgwConnector.OpenIdConfig.AuthorizationEndpoint, diAuthorizationRequest!);
				return Redirect(idGatewayDIAuthUri);
			}
			catch (UnifiedException ex)
			{
				return Redirect($"{diAuthorizationRequest!.RedirectUri}?error={ex.Error}&error_description={ex.ErrorDescription}");
			}
			catch
			{
				return Redirect($"{diAuthorizationRequest!.RedirectUri}?error=server_error");
			}
		}

		[HttpGet("api/v1/mobileidcallback")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> MobileidCallBack([FromQuery(Name = "state")] string? stateNew, string? code, string? error, string? error_description)
		{
			string? cookiesStateNew = GetDICookieStateNew();
			DIAuthorizationRequest? diAuthorizationRequest = await GetDiAuthRequestByStateAsync(cookiesStateNew, stateNew);
			try
			{
				await diRequestValidationService.ValidateIdgwCallBackAsync(diAuthorizationRequest!, code, error, error_description);

				diAuthorizationRequest!.Code = code!;
				await AggregatorContext.SaveAsync(ctx => ctx.DIAuthorizationRequests!.Update(diAuthorizationRequest));

				var redirectUri = diAuthorizationService.CreateAggregatorRedirectUri(diAuthorizationRequest!, code, error, error_description);
				ClearCookieAfterDIAuthorized();

				return Redirect(redirectUri);
			}
			catch (UnifiedException ex)
			{
				return Redirect($"{diAuthorizationRequest!.RedirectUri}?error={ex.Error}&error_description={ex.ErrorDescription}");
			}
			catch
			{
				return Redirect($"{diAuthorizationRequest!.RedirectUri}?error=server_error");
			}
		}

		void SetDICookieStateNew(string stateNew)
		{
			if (Request.Cookies.ContainsKey("diauthorize"))
				Response.Cookies.Delete("diauthorize");
			Response.Cookies.Append("diauthorize", stateNew);
		}

		string? GetDICookieStateNew()
		{
			Request.Cookies.TryGetValue("diauthorize", out var stateNew);
			return stateNew;
		}

		void ClearCookieAfterDIAuthorized()
		{
			if (Request.Cookies.ContainsKey("diauthorize")) 
				Response.Cookies.Delete("diauthorize");
		}

		async Task<DIAuthorizationRequest> GetDiAuthRequestByStateAsync(string? cookiesStateNew, string? stateNew)
		{
			if (!string.IsNullOrEmpty(cookiesStateNew))
				return await AggregatorContext.QueryAsync(ctx => ctx.DIAuthorizationRequests!.FirstAsync(l => l.StateNew == cookiesStateNew!));
			return await AggregatorContext.QueryAsync(ctx => ctx.DIAuthorizationRequests!.FirstAsync(l => l.StateNew == stateNew!));
		}

		async Task<DIAuthorizationRequest> GetDiAuthRequestByDcidAsync(string? cookiesStateNew, string? dcid)
		{
			if (!string.IsNullOrEmpty(cookiesStateNew))
				return await AggregatorContext.QueryAsync(ctx => ctx.DIAuthorizationRequests!.FirstAsync(l => l.StateNew == cookiesStateNew!));
			return await AggregatorContext.QueryAsync(ctx => ctx.DIAuthorizationRequests!.FirstAsync(l => l.Dcid == dcid!));
		}
	}
}