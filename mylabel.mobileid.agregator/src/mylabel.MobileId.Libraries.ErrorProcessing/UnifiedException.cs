using System;
using System.Net;

namespace Beeline.MobileId.Libraries.ErrorProcessing
{
	public class UnifiedException : Exception
	{
		public UnifiedException(HttpStatusCode httpStatusCode, string error, string errorDescription)
		{
			HttpStatusCode = httpStatusCode;
			Error = error;
			ErrorDescription = errorDescription;
		}

		public UnifiedException(HttpStatusCode httpStatusCode, string error)
		{
			HttpStatusCode = httpStatusCode;
			Error = error;
		}

		public HttpStatusCode HttpStatusCode { get; }

		public string Error { get; }

		public string? ErrorDescription { get; }
	}
}