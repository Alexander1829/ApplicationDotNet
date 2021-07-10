using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace mylabel.MobileId.Aggregator.Common.Logging
{
	public class NLogDelegate
	{
		public static async Task LogRequest(ILogger<ApiRequestResponseMiddleware> logger, Guid guid, HttpRequest request, Tuple<PathString, Func<string, string>>[] requestLogFilter = null)
		{
			request.EnableBuffering();

			var buffer = new byte[Convert.ToInt32(request.ContentLength)];

			await request.Body.ReadAsync(buffer, 0, buffer.Length);
			request.Body.Seek(0, SeekOrigin.Begin);

			var bodyAsText = Encoding.UTF8.GetString(buffer);

			if (MultipartRequestHelper.IsMultipartContentType(request.ContentType))
			{
				MediaTypeHeaderValue.TryParse(request.ContentType, out var h);
				if (h != null)
				{
					try
					{
						var boundary = MultipartRequestHelper.GetBoundary(h, int.MaxValue);
						bodyAsText = Regex.Replace(bodyAsText, @$"{boundary}.*", $"{boundary} replaced by logger", RegexOptions.Multiline | RegexOptions.Singleline);
					}
					catch
					{
						// do nothing
					}
				}
			}

			var logBody = bodyAsText.Clone().ToString();
			foreach (var filter in requestLogFilter)
			{
				var (path, func) = filter;
				if (request.Path.ToString().StartsWith(path.ToString(), StringComparison.InvariantCultureIgnoreCase))
					logBody = func?.Invoke(logBody);
			}

			var obj = new
			{
				scheme = request.Scheme,
				uri = $"{request.Host}{request.Path}",
				method = request.Method,
				query = request.QueryString.HasValue ? request.QueryString.ToString() : "",
				headers = request.Headers,
				remoteIp = $"{request.HttpContext.Connection.RemoteIpAddress}:{request.HttpContext.Connection.RemotePort}",
				body = logBody
			};

			logger?.LogInformation($"Incoming request: {JsonSerializer.Serialize(obj)}");
		}

		public static void LogResponse(ILogger<ApiRequestResponseMiddleware> logger,
									   Guid guid,
									   HttpRequest request,
									   HttpResponse response,
									   string responseBody,
									   TimeSpan elapsedTime)
		{
			var requestDuration = elapsedTime.ToString();
			var obj = new
			{
				status = response.StatusCode,
				headers = response.Headers,
				body = responseBody,
				totalTime = requestDuration
			};			
			var extData = JsonSerializer.Serialize(obj);
			logger?.LogInformation("Outgoing response: {extData}. Duration: {requestDuration}", extData, requestDuration);
		}
	}
}
