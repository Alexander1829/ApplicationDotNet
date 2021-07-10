using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace mylabel.MobileId.Aggregator.Common.Logging
{
	public static class ApplicationBuilderExtension
	{
		public static IApplicationBuilder UseNLogHelper(this IApplicationBuilder app)
		{
			return app.UseMiddleware<NLogHelperMiddleware>();
		}

		public static IApplicationBuilder MapRequesResponseLogger(this IApplicationBuilder app, PathString[] path, PathString[] exclude, PathString[] skipResponseBody = null, Tuple<PathString, Func<string, string>>[] requestLogFilter = null)
		{
			ApiRequestResponseMiddleware.RegisterRequestHandler(NLogDelegate.LogRequest);
			ApiRequestResponseMiddleware.RegisterResponseHandler(NLogDelegate.LogResponse);

			skipResponseBody ??= new PathString[] { };
			requestLogFilter ??= new Tuple<PathString, Func<string, string>>[] { };

			return app.UseMiddleware<ApiRequestResponseMiddleware>(path, exclude, skipResponseBody, requestLogFilter);
		}
	}
}
