using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace mylabel.MobileId.Aggregator.Common.Logging
{
	public class ApiRequestResponseMiddleware
	{
		public delegate Task RequestHandler(ILogger<ApiRequestResponseMiddleware> logger, Guid guid, HttpRequest request, Tuple<PathString, Func<string, string>>[] requestLogFilter = null);
		public delegate void ResponseHandler(ILogger<ApiRequestResponseMiddleware> logger, Guid guid, HttpRequest request, HttpResponse response, string responseBody, TimeSpan elapsedTime);

		private readonly RequestDelegate _next;
		private readonly PathString[] _path;
		private readonly PathString[] _exclude;
		private readonly PathString[] _skipResponseBody;
		private readonly Tuple<PathString, Func<string, string>>[] _requestLogFilter;
		private static RequestHandler _requestHandler;
		private static ResponseHandler _responseHandler;

		public ApiRequestResponseMiddleware(RequestDelegate next, PathString[] path, PathString[] exclude, PathString[] skipResponseBody, Tuple<PathString, Func<string, string>>[] requestLogFilter = null)
		{
			_next = next;
			_path = path;
			_exclude = exclude;
			_requestLogFilter = requestLogFilter;
			_skipResponseBody = skipResponseBody;
		}

		public static void RegisterRequestHandler(RequestHandler req)
		{
			_requestHandler += req;
		}

		public static void UnregisterRequestHandler(RequestHandler req)
		{
			if (_requestHandler != null)
				_requestHandler -= req;
		}

		public static void RegisterResponseHandler(ResponseHandler res)
		{
			_responseHandler += res;
		}

		public static void UnregisterResponseHandler(ResponseHandler res)
		{
			if (_responseHandler != null)
				_responseHandler -= res;
		}

		public async Task Invoke(HttpContext context, ILogger<ApiRequestResponseMiddleware> logger)
		{
			var requestPath = context.Request.Path.ToString();
			if (
				!_path.Any(x => requestPath.StartsWith(x.ToString(), StringComparison.InvariantCultureIgnoreCase))
				|| _exclude.Any(x => requestPath.StartsWith(x.ToString(), StringComparison.InvariantCultureIgnoreCase))
			)
			{
				await _next(context);
				return;
			}

			var guid = LogHelper.SetAndGetCurrentActivityId();
			var time = Stopwatch.StartNew();

			if (_requestHandler != null)
				await _requestHandler?.Invoke(logger, guid, context.Request, _requestLogFilter);

			var skipBody = _skipResponseBody?.Any(x =>
				requestPath.StartsWith(x.ToString(), StringComparison.InvariantCultureIgnoreCase)) ?? false;

			var originalBodyStream = context.Response.Body;

			using (var responseBody = new MemoryStream())
			{
				context.Response.Body = responseBody;

				await _next(context);

				var responseBodyBytes = responseBody.ToArray();

				context.Response.Body = originalBodyStream;

				if (skipBody && !(context.Response.StatusCode >= 200 && context.Response.StatusCode <= 299))
					skipBody = false;

				time.Stop();
				if (skipBody == false)
				{
					if (_responseHandler != null)
					{
						var responseBodyContent = Encoding.UTF8.GetString(responseBodyBytes);
						foreach (var method in _responseHandler.GetInvocationList())
						{
							try
							{
								method.DynamicInvoke(logger, guid, context.Request, context.Response, responseBodyContent, time.Elapsed);
							}
							catch (TargetInvocationException ex)
							{
								if (ex.InnerException != null)
									responseBodyContent = ex.InnerException.Message;
							}
						}
					}
				}
				else
				{
					foreach (var method in _responseHandler.GetInvocationList())
					{
						try
						{
							method.DynamicInvoke(logger, guid, context.Request, context.Response, "replaced by logger", time.Elapsed);
						}
						catch
						{
							// do nothing
						}
					}
				}

				await context.Response.Body.WriteAsync(responseBodyBytes);
			}
		}
	}
}
