using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace mylabel.MobileId.Aggregator.Common
{
	public static class OAuth2ErrorDetails
	{
		public static HttpStatusCode GetCode(OAuth2Error error) => errors[error].StatusCode;

		public static string GetText(OAuth2Error error) => errors[error].ErrorText;

		public static OAuth2Error GetError(string text) => reverse.ContainsKey(text) ? reverse[text] : OAuth2Error.ServerError;

		static Dictionary<OAuth2Error, (string ErrorText, HttpStatusCode StatusCode)> errors = new()
		{
			[OAuth2Error.AccessDenied] = ("access_denied", HttpStatusCode.Forbidden),
			[OAuth2Error.InvalidGrant] = ("invalid_grant", HttpStatusCode.BadRequest),
			[OAuth2Error.InvalidRequest] = ("invalid_request", HttpStatusCode.BadRequest),
			[OAuth2Error.InvalidScope] = ("invalid_scope", HttpStatusCode.BadRequest),
			[OAuth2Error.ServerError] = ("server_error", HttpStatusCode.InternalServerError),
			[OAuth2Error.TemporarilyUnavailable] = ("temporarily_unavailable", HttpStatusCode.ServiceUnavailable),
			[OAuth2Error.UnauthorizedClient] = ("unauthorized_client", HttpStatusCode.Unauthorized),
			[OAuth2Error.UnsupportedGrantType] = ("unsupported_grant_type", HttpStatusCode.BadRequest),
			[OAuth2Error.UnsupportedResponseType] = ("unsupported_response_type", HttpStatusCode.BadRequest)
		};

		static Dictionary<string, OAuth2Error> reverse = errors.ToDictionary(p => p.Value.ErrorText, p => p.Key);
	}
}