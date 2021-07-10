using System;
using System.Collections.Immutable;
using System.Linq;
using mylabel.MobileId.Aggregator.Common;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace mylabel.MobileId.Aggregator.BusinessLogic
{
	public class SIRequestValidationService
	{
		public (string Msisdn, string AcrValues) ValidateAndGetJwtPayload(string? clientId, string? scope, string? request)
		{
			foreach (var parameter in new (string Name, string? Value)[] {
				(OpenIdConnectParameterNames.ClientId, clientId),
				(OpenIdConnectParameterNames.Scope, scope),
				(MobileConnectParameterNames.Request, request) })
			{
				if (parameter.Value == null)
					throw new UnifiedException(OAuth2Error.InvalidRequest, $"Mandatory parameter {parameter.Name} is missing");
				if (string.IsNullOrWhiteSpace(parameter.Value))
					throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(parameter.Name));
			}

			var scopes = scope!.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToImmutableHashSet();
			if (scopes.Count < 2 || !scopes.Contains(OpenIdConnectScope.OpenId))
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(OpenIdConnectParameterNames.Scope));

			var claims = new JsonWebToken(request).Claims.ToImmutableDictionary(c => c.Type, c => c.Value);

			if (new[]{
				OpenIdConnectParameterNames.AcrValues,
				OpenIdConnectParameterNames.ClientId,
				OpenIdConnectParameterNames.LoginHint,
				OpenIdConnectParameterNames.Scope,
				MobileConnectParameterNames.ClientNotificationToken,
				MobileConnectParameterNames.NotificationUri}.Any(p => !claims.ContainsKey(p) || string.IsNullOrWhiteSpace(claims[p]))
				|| claims[OpenIdConnectParameterNames.ClientId] != clientId
				|| !claims[OpenIdConnectParameterNames.LoginHint].StartsWith("MSISDN:")
				|| !scopes.SequenceEqual(claims[OpenIdConnectParameterNames.Scope].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToImmutableHashSet()))
			{
				throw new UnifiedException(OAuth2Error.InvalidRequest, GetInvalidDescription(MobileConnectParameterNames.Request));
			}

			return (claims[OpenIdConnectParameterNames.LoginHint].Split(":", StringSplitOptions.RemoveEmptyEntries).Last()
				, claims[OpenIdConnectParameterNames.AcrValues]);
		}

		string GetInvalidDescription(string parameterName) => $"Mandatory parameter {parameterName} is invalid";
	}
}