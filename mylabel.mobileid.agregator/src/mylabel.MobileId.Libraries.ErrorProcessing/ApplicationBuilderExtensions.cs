using System.Collections.Generic;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Beeline.MobileId.Libraries.ErrorProcessing
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseErrorProcessing(this IApplicationBuilder applicationBuilder) => applicationBuilder.UseExceptionHandler(ab => ab.Run(httpContext =>
		{
			httpContext.Response.ContentType = MediaTypeNames.Application.Json;

			if (httpContext.Features.Get<IExceptionHandlerFeature>().Error is UnifiedException unifiedException)
			{
				httpContext.Response.StatusCode = (int)unifiedException.HttpStatusCode;

				Dictionary<string, string> response = new() { [OpenIdConnectParameterNames.Error] = unifiedException.Error };

				if (!string.IsNullOrWhiteSpace(unifiedException.ErrorDescription))
					response[OpenIdConnectParameterNames.ErrorDescription] = unifiedException.ErrorDescription;

				return httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
			}
			else
				return httpContext.Response.WriteAsync(JsonSerializer.Serialize(new Dictionary<string, string>
				{
					[OpenIdConnectParameterNames.Error] = OAuth2Errors.ServerError
				}));
		}));
	}
}