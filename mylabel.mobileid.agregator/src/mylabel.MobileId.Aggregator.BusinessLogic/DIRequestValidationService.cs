using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace mylabel.MobileId.Aggregator.BusinessLogic
{
	public class DIRequestValidationService
	{
		public void ValidateWithRedirect(DIAuthorizationRequest request)
		{
			if (string.IsNullOrEmpty(request.ClientId))
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.ClientId));
			if (string.IsNullOrEmpty(request.Scope))
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.Scope));
			if (string.IsNullOrEmpty(request.ResponseType))
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.ResponseType));
			if (string.IsNullOrEmpty(request.Nonce))
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.Nonce));
			if (string.IsNullOrEmpty(request.Version))
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(MobileConnectParameterNames.Version));
			if (string.IsNullOrEmpty(request.State))
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.State));

			var scopes = request.Scope!.Split(" ", StringSplitOptions.RemoveEmptyEntries);
			if (scopes.Distinct().Count() < 2 || !scopes.Contains("openid"))
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.Scope));
			if (request.ResponseType != "code")
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.ResponseType));

			var loginHint = request.LoginHint;
			if (!string.IsNullOrEmpty(loginHint))
			{
				if (loginHint.Contains("ENCR_MSISDN:"))
				{
					if (loginHint.Substring(loginHint.IndexOf(":") + 1).Length == 0)
						throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.LoginHint));
				}
				else if (loginHint.Contains("MSISDN:"))
				{
					if (!loginHint.Substring(loginHint.IndexOf(":") + 1).All(char.IsDigit))
						throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.LoginHint));
				}				
				else if (loginHint.Contains("PCR:"))
					throw new UnifiedException(OAuth2Error.InvalidRequest, "");
				else
					throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.LoginHint));
			}
			try
			{
				var acrValues = request.AcrValues!.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
				var acrValuesAllowed = new int[] { 2, 3, 4 };
				if (acrValues.Length == 0 || acrValues.Intersect(acrValuesAllowed).Count() != acrValues.Length)
					throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.AcrValues));
			}
			catch
			{
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.AcrValues));
			}
		}

		public async Task ValidateDiscoCallBackAsync(DIAuthorizationRequest request, string? error, string? error_description)
		{
			if (!string.IsNullOrEmpty(error))
			{
				var diAuthorizationRequest = await AggregatorContext.QueryAsync(ctx => ctx.DIAuthorizationRequests.FirstAsync(r => r.StateNew == request.StateNew));
				diAuthorizationRequest!.Error = error;
				diAuthorizationRequest!.ErrorDescription = error_description;
				diAuthorizationRequest!.ErrorAt = DateTimeOffset.Now;
				await AggregatorContext.SaveAsync(ctx => ctx.DIAuthorizationRequests!.Update(diAuthorizationRequest));

				throw new UnifiedException(OAuth2ErrorDetails.GetError(error), error_description);
			}
		}

		public async Task ValidateIdgwCallBackAsync(DIAuthorizationRequest request, string? code, string? error, string? error_description)
		{
			if (!string.IsNullOrEmpty(error))
			{
				var diAuthorizationRequest = await AggregatorContext.QueryAsync(ctx => ctx.DIAuthorizationRequests.FirstAsync(r => r.StateNew == request.StateNew));
				diAuthorizationRequest!.Error = error;
				diAuthorizationRequest!.ErrorDescription = error_description;
				diAuthorizationRequest!.ErrorAt = DateTimeOffset.Now;
				await AggregatorContext.SaveAsync(ctx => ctx.DIAuthorizationRequests!.Update(diAuthorizationRequest));

				throw new UnifiedException(OAuth2ErrorDetails.GetError(error), error_description);
			}

			if (string.IsNullOrEmpty(code))
			{
				var diAuthorizationRequest = await AggregatorContext.QueryAsync(ctx => ctx.DIAuthorizationRequests.FirstAsync(r => r.StateNew == request.StateNew));
				diAuthorizationRequest!.Error = "Empty parameter code";
				diAuthorizationRequest!.ErrorDescription = "Empty parameter code";
				diAuthorizationRequest!.ErrorAt = DateTimeOffset.Now;
				await AggregatorContext.SaveAsync(ctx => ctx.DIAuthorizationRequests!.Update(diAuthorizationRequest));

				throw new UnifiedException(OAuth2Error.ServerError, "Empty parameter code");
			}
		}

		public async Task<ServiceProvider> ValidateWithoutRedirectAsync(string? clientId, string? redirectUri)
		{
			if (string.IsNullOrEmpty(clientId))
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, GetMissingDescription(OpenIdConnectParameterNames.ClientId));

			if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri))
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, GetMissingOrInvalidDescription(OpenIdConnectParameterNames.RedirectUri));

			var serviceProvider = await AggregatorContext.QueryAsync(ctx => ctx.ServiceProviders
				.Include(p => p.AllowedRedirectUris)
				.Include(p => p.AllowedNotificationUris)
				.FirstOrDefaultAsync(p => p.ClientIdOnAggregator == clientId!));
			if (serviceProvider == null)
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, NotAuthorized);

			if (!serviceProvider.AllowedRedirectUris!.Any(u => u.Value == redirectUri!))
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, GetMissingOrInvalidDescription(OpenIdConnectParameterNames.RedirectUri));
			return serviceProvider;
		}

		private string GetInvalidDescription(string parameterName) => $"Mandatory parameter {parameterName} is invalid";

		private string GetMissingDescription(string parameterName) => $"Mandatory parameter {parameterName} is missing";

		private string GetMissingOrInvalidDescription(string parameterName) => $"Mandatory parameter {parameterName} is missing or invalid";

		private const string NotAuthorized = "The client is not authorized to request an authorization code";
	}
}
