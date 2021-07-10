using System;
using System.Linq;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.BusinessLogic.Dal;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Db;
using mylabel.MobileId.Aggregator.Integrations.Discovery.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace mylabel.MobileId.Aggregator.BusinessLogic
{
	public class DIRequestAuthorizationService
	{
		Settings settings;

		public DIRequestAuthorizationService(IOptions<Settings> settings)
		{
			this.settings = settings.Value;
		}

		public string GetOpenidConfigUri(DiscoveryFullResponse discoClient)
		{
			return discoClient.Response.APIs.OperatorApiInfo.Link.Where(l => l.Rel == "openid-configuration").First().Uri;
		}

		public string? GetPremiumInfoUri(DiscoveryFullResponse discoClient)
		{
			return discoClient.Response.APIs.OperatorApiInfo.Link.Where(l => l.Rel == "userinfo").FirstOrDefault()?.Uri;
		}

		public async Task<DiscoveryClientDetails> GetDiscoSettingsByClientIdAsync(string clientIdOnAggregator)
		{
			var serviceProvider = await AggregatorContext.QueryAsync(ctx => ctx.ServiceProviders
				.Include(p => p.AllowedNotificationUris)
				.Include(p => p.Discoveries).ThenInclude(d => d.DiscoveryService)
				.FirstOrDefaultAsync(p => p.ClientIdOnAggregator == clientIdOnAggregator));
			if (serviceProvider == null)
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, GetInvalidDescription(OpenIdConnectParameterNames.ClientId));

			var disco = serviceProvider.Discoveries!.First(d => d.IsEnabled);

			return new DiscoveryClientDetails(disco.DiscoveryService!.Uri!, disco.ClientIdOnDiscovery!, disco.ClientSecretOnDiscovery!, disco.RedirectUri!);
		}

		public string CreateIdGatewayDIAuthUri(string authorizationEndpoint, DIAuthorizationRequest q)
		{
			string idgwRedirectUri = settings.DIAuthorizeIdgwCallbackUri!;

			var idgwAuthRequest = $"?" +
			 $"client_id={q.IdgwClientId!}" +
			 $"&redirect_uri={Uri.EscapeDataString(idgwRedirectUri!)}" +
			 $"&scope={Uri.EscapeDataString(q.Scope!)}" +
			 $"&response_type={q.ResponseType!}" +
			 $"&acr_values={q.AcrValues!}" +
			 $"&state={q.StateNew!}" +
			 $"&nonce={q.Nonce!}" +
			 $"&version={q.Version!}" +
			 $"&login_hint={q.LoginHint!}";

			if (!string.IsNullOrEmpty(q.ClientName))
				idgwAuthRequest += $"&client_name={q.ClientName}";
			if (!string.IsNullOrEmpty(q.Display))
				idgwAuthRequest += $"&display={q.Display}";

			var redirectUri = $"{authorizationEndpoint}{idgwAuthRequest}";
			return redirectUri;
		}

		public string CreateAggregatorRedirectUri(DIAuthorizationRequest diAuthorizeRequest, string? code, string? error, string? error_description)
		{
			if(!string.IsNullOrEmpty(error))
				return $"{diAuthorizeRequest.RedirectUri}?state={diAuthorizeRequest.State!}&error={error}&error_description={error_description}";
			return $"{diAuthorizeRequest.RedirectUri}?state={diAuthorizeRequest.State!}&code={code}";
		}

		string GetInvalidDescription(string parameterName) => $"Mandatory parameter {parameterName} is invalid";
	}
}