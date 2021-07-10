using System.Linq;
using System.Text.Json;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace mylabel.MobileId.Aggregator.BusinessLogic
{
	public class PremiumInfoValidationService
	{
		Settings settings;

		public PremiumInfoValidationService(IOptions<Settings> settings)
		{
			this.settings = settings.Value;
		}

		const string BearerWithSpace = "Bearer ";

		public string CheckAndGetAccessTokenOnAggregator(HttpRequest Request)
		{
			var authHeaderValue = Request.Headers[HeaderNames.Authorization].FirstOrDefault();
			if (string.IsNullOrEmpty(authHeaderValue) || authHeaderValue.IndexOf(BearerWithSpace) != 0)
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, "No valid token found");
			
			var token = authHeaderValue.Substring(authHeaderValue.IndexOf(BearerWithSpace) + BearerWithSpace.Length);

			var aggregatorSignJwk = HelperJwks.CreateJwk(settings.PrivateKey!, JsonWebKeyUseNames.Sig);
			var jwks = new JsonWebKeySet();
			jwks.Keys.Add(aggregatorSignJwk);
			string jwksString = JsonSerializer.Serialize(jwks, options: new JsonSerializerOptions(){ 
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
			if (!JwtSignatureValidator.Validate(token, jwksString!, out var claims))
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, "Token validation failed");

			return token;
		}
	}
}
