using System;

namespace mylabel.MobileId.Aggregator.Cryptography
{
	public static class UriSafeBase64
	{
		public static string Encode(byte[] bytes) => Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');

		public static byte[] Decode(string base64)
		{
			base64 = base64.Replace('-', '+').Replace('_', '/');

			base64 = (base64.Length % 4) switch
			{
				0 => base64,
				1 => throw new FormatException("The input is not a valid Base-64 string."),
				2 => $"{base64}==",
				3 => $"{base64}="
			};

			return Convert.FromBase64String(base64);
		}
	}
}