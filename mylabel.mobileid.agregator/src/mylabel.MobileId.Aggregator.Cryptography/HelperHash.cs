using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace mylabel.MobileId.Aggregator.Cryptography
{
	public class HelperHash
	{
		public static string AtHashString(string input)
		{
			using var sha256 = SHA256.Create();
			var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(input));
			return UriSafeBase64.Encode(hash.Take(hash.Length / 2).ToArray());
		}

		public static string HashString(string input)
		{
			using var sha256 = SHA256.Create();
			var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(input));
			return Convert.ToBase64String(hash);
		}
	}
}
