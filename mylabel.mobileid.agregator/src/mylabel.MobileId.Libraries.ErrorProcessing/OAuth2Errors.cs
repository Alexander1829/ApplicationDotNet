using System.Collections.ObjectModel;

namespace Beeline.MobileId.Libraries.ErrorProcessing
{
	///<summary>https://tools.ietf.org/html/rfc6749</summary>
	public static class OAuth2Errors
	{
		public static ReadOnlyCollection<string> All { get; } = new ReadOnlyCollection<string>(new[] {
			AccessDenied, InvalidClient, InvalidGrant, InvalidRequest, InvalidScope, ServerError, TemporarilyUnavailable, UnauthorizedClient, UnsupportedGrantType, UnsupportedResponseType });

		public const string AccessDenied = "access_denied";

		public const string InvalidClient = "invalid_client";

		public const string InvalidGrant = "invalid_grant";

		public const string InvalidRequest = "invalid_request";

		public const string InvalidScope = "invalid_scope";

		public const string ServerError = "server_error";

		public const string TemporarilyUnavailable = "temporarily_unavailable";

		public const string UnauthorizedClient = "unauthorized_client";

		public const string UnsupportedGrantType = "unsupported_grant_type";

		public const string UnsupportedResponseType = "unsupported_response_type";
	}
}