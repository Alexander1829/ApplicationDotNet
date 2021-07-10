using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace mylabel.MobileId.Aggregator.Common
{
	public static class HelperJwks
	{
		public static JsonWebKey CreateJwk(string privateKey, string use)
		{
			if (!new string[] { JsonWebKeyUseNames.Sig, JsonWebKeyUseNames.Enc }.Any(u => u == use))
				throw new Exception("Wrong jwk use");

			using var rsa = RSA.Create();

			rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

			var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(new(rsa));

			jwk.Alg = SecurityAlgorithms.RsaSha256;
			jwk.Kid = HelperHash.HashString(privateKey);
			jwk.Use = use;

			return jwk;
		}
	}
}
