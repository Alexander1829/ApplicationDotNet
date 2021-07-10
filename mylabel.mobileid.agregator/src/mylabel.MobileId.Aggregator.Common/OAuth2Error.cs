namespace mylabel.MobileId.Aggregator.Common
{
	public enum OAuth2Error
	{
		InvalidRequest,
		UnauthorizedClient,
		AccessDenied,
		UnsupportedResponseType,
		InvalidScope,
		ServerError,
		TemporarilyUnavailable,

		// beyond oauth2:

		InvalidGrant,
		UnsupportedGrantType
	}
}