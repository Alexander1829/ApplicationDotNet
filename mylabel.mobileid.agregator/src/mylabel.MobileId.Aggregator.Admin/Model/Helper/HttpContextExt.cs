using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace mylabel.MobileId.Aggregator.Admin.Model.Helper
{
	public static class HttpContextExt
	{
		public static string CreateLogMessage(this HttpContext httpContext, string subMessage, object? @params = null)
		{
			var indentity = httpContext?.User?.Identity as ClaimsIdentity;
			var userName = indentity?.Claims?.FirstOrDefault(l => l.Type == ClaimTypes.Name)?.Value;

			object result;
			if (userName != null)
				if (@params != null)
					result = new
					{
						User = userName,
						SubMessage = subMessage,
						Params = @params
					};
				else
					result = new
					{
						User = userName,
						SubMessage = subMessage
					};
			else
			{
				if (@params != null)
					result = new
					{
						SubMessage = subMessage,
						Params = @params
					};
				else
					result = new
					{
						SubMessage = subMessage
					};
			}
			return JsonSerializer.Serialize(result);
		}
	}
}
