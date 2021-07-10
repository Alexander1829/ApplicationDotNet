using System;

namespace mylabel.MobileId.Aggregator.Common
{
	public class UnifiedException : Exception
	{
		public UnifiedException(OAuth2Error error)
		{
			Error = error;
		}

		public UnifiedException(OAuth2Error error, string? errorDescription)
		{
			Error = error;
			ErrorDescription = errorDescription;
		}

		public OAuth2Error Error { get; }

		public string? ErrorDescription { get; }
	}
}