using System.Collections.Generic;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using static System.Linq.Enumerable;

namespace mylabel.MobileId.Aggregator.Cryptography
{
	public static class JwtSignatureValidator
	{
		public static bool Validate(string jwtToValidate, string jwks, out IDictionary<string, object>? claims)
		{
			JsonWebKeySet jwksObject = new(jwks);

			var tokenValidationResult = new JsonWebTokenHandler().ValidateToken(
				jwtToValidate,
				new()
				{
					IssuerSigningKey = jwksObject.GetSigningKeys().First(),
					CryptoProviderFactory = new() { CacheSignatureProviders = false },
					ValidateAudience = false,
					ValidateIssuer = false,
					ValidateLifetime = false
				});

			if (!tokenValidationResult.IsValid)
			{
				claims = null;

				return false;
			}

			claims = tokenValidationResult.Claims;

			return true;
		}
	}
}