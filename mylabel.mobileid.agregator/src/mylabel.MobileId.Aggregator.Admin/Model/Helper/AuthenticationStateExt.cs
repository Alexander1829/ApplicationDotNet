using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace mylabel.MobileId.Aggregator.Admin.Model.Helper
{
	public static class AuthenticationStateExt
	{
		public static bool IsAdmin(this AuthenticationState authenticationState)
		{
			var indentity = authenticationState?.User?.Identity as ClaimsIdentity;
			if (indentity != null)
			{
				var userRole = indentity?.Claims?.FirstOrDefault(l => l.Type == ClaimTypes.Role)?.Value;
				return userRole == "Admin";
			}
			return false;
		}

		public static string? GetUserName(this AuthenticationState authenticationState)
		{
			var indentity = authenticationState?.User?.Identity as ClaimsIdentity;
			if (indentity != null)
				return indentity?.Claims?.FirstOrDefault(l => l.Type == ClaimTypes.Name)?.Value;
			return null;
		}

		public static string CreateLogMessage(this AuthenticationState authenticationState, string subMessage, object? @params = null)
		{
			var indentity = authenticationState?.User?.Identity as ClaimsIdentity;
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
