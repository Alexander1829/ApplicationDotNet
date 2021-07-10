using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using mylabel.MobileId.Aggregator.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;

namespace mylabel.MobileId.Aggregator.BusinessLogic.MobileConnect
{
	public class JwtAccessToken
	{
		public JwtAccessToken(string source, string? issuerJwks = null)
		{
			if (new JsonWebTokenHandler().CanReadToken(source))
			{
				JsonWebToken jsonWebToken = new(source);

				Aud = jsonWebToken.TryGetPayloadValue(JwtRegisteredClaimNames.Aud, out string aud) ? aud : null;
				Azp = jsonWebToken.TryGetPayloadValue(JwtRegisteredClaimNames.Azp, out string azp) ? azp : null;
				Exp = jsonWebToken.TryGetPayloadValue(JwtRegisteredClaimNames.Exp, out long exp) ? exp : null;
				Iat = jsonWebToken.TryGetPayloadValue(JwtRegisteredClaimNames.Iat, out long iat) ? iat : null;
				Iss = jsonWebToken.TryGetPayloadValue(JwtRegisteredClaimNames.Iss, out string iss) ? iss : null;
				Jti = jsonWebToken.TryGetPayloadValue(JwtRegisteredClaimNames.Jti, out Guid jti) ? jti : null;
				Sub = jsonWebToken.TryGetPayloadValue(JwtRegisteredClaimNames.Sub, out string sub) ? sub : null;

				if (issuerJwks != null)
					SourceSignatureIsValid = JwtSignatureValidator.Validate(source, issuerJwks, out _);
				IsJwt = true;
			}
		}

		public bool IsJwt { get; }
		public string? Aud { get; set; }
		public string? Azp { get; set; }
		public long? Exp { get; set; }
		public long? Iat { get; set; }
		public string? Iss { get; set; }
		public Guid? Jti { get; set; }
		public string? Sub { get; set; }

		public bool? SourceSignatureIsValid { get; }

		public (string Jwt, string AtHash) SignCurrentState(string privateKey, string? kid = null)
		{
			var jwt = SignedJwtCreator.Create(new Dictionary<string, object?>()
			{
				[JwtRegisteredClaimNames.Aud] = Aud,
				[JwtRegisteredClaimNames.Azp] = Azp,
				[JwtRegisteredClaimNames.Exp] = Exp,
				[JwtRegisteredClaimNames.Iat] = Iat,
				[JwtRegisteredClaimNames.Iss] = Iss,
				[JwtRegisteredClaimNames.Jti] = Jti,
				[JwtRegisteredClaimNames.Sub] = Sub
			}.Where(p => p.Value != null).ToImmutableDictionary(p => p.Key, p => p.Value!), privateKey, kid);

			using var sha256 = SHA256.Create();
			var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(jwt));
			var atHash = UriSafeBase64.Encode(hash.Take(hash.Length / 2).ToArray());

			return (jwt, atHash);
		}
	}
}