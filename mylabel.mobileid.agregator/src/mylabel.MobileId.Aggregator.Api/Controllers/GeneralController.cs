using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace mylabel.MobileId.Aggregator.Api.Controllers
{
	[ApiController]
	public class GeneralController : ControllerBase
	{
		Settings settings;

		public GeneralController(IOptions<Settings> settings)
		{
			this.settings = settings.Value;
		}

		[HttpGet("jwks")]
		[Produces("application/json")]
		public object Jwks()
		{
			MobileConnectJwk jwk = new(HelperJwks.CreateJwk(settings.PrivateKey!, JsonWebKeyUseNames.Sig));

			MobileConnectJwks jwks = new();
			jwks.Keys.Add(jwk);

			return jwks;
		}

		//Смотрел в интернете как сделать, чтобы свагер видел эндпоинты healtcheck'а - из коробки решения не нашёл
		[HttpGet("health")]
		public async Task<string> Health()
		{
			return await CallHealthCheck();
		}

		[HttpGet("health/db")]
		public async Task<string> HealthDb()
		{
			return await CallHealthCheck("db");
		}
		
		async Task<string> CallHealthCheck(string? tag = null) 
		{
			using var httpClient = new HttpClient();
			var uri = Request.Scheme + "://" + Request.Host + "/p-health";
			uri = tag != null ? 
				uri + "/" + tag:
				uri;
			var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);

			var response = await httpClient.SendAsync(httpRequest);
			Response.StatusCode = (int)response.StatusCode;
			return await response.Content.ReadAsStringAsync();
		}
	}
}
