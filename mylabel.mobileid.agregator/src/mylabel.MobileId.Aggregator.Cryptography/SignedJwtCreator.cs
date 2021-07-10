using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace mylabel.MobileId.Aggregator.Cryptography
{
	public static class SignedJwtCreator
	{
		public static string Create(IDictionary<string, object> claims, string base64EncodedPrivateKey, string? kid = null)
		{
			using var rsa = RSA.Create();

			rsa.ImportRSAPrivateKey(Convert.FromBase64String(base64EncodedPrivateKey), out _);

			SigningCredentials signingCredentials = new(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256) { CryptoProviderFactory = new() { CacheSignatureProviders = false } };

			signingCredentials.Key.KeyId = !string.IsNullOrEmpty(kid) ? kid : HelperHash.HashString(base64EncodedPrivateKey);

			SecurityTokenDescriptor descriptor = new()
			{
				Claims = claims,
				SigningCredentials = signingCredentials
			};

			return new JsonWebTokenHandler().CreateToken(descriptor);
		}

		public static string Create(string payload, string base64EncodedPrivateKey, string? kid = null)
		{
			using var rsa = RSA.Create();

			rsa.ImportRSAPrivateKey(Convert.FromBase64String(base64EncodedPrivateKey), out _);

			SigningCredentials signingCredentials = new(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256) { CryptoProviderFactory = new() { CacheSignatureProviders = false } };

			signingCredentials.Key.KeyId = !string.IsNullOrEmpty(kid) ? kid : HelperHash.HashString(base64EncodedPrivateKey);

			return new JsonWebTokenHandler().CreateToken(payload, signingCredentials);
		}
	}
}