using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace mylabel.MobileId.Aggregator.BusinessLogic
{
	public class DIMCTokenService
	{
		const string Basic = "Basic";

		public async Task EnsureIncomingCredentialsAreValidAsync(StringValues authHeaderValues)
		{
			var idHashPairs = authHeaderValues
				.Select(v => v.Split(" "))
				.Where(v => v.First() == Basic)
				.Select(v => v.Last())
				.Select(credential =>
				{
					var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(credential));
					var index = decoded.IndexOf(":");
					using var sha256 = SHA256.Create();
					var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(decoded[(index + 1)..])));
					return (ClientId: decoded[..index], ClientSecretHash: passwordHash);
				}).ToImmutableArray();

			foreach (var pair in idHashPairs)
			{
				if (await AggregatorContext.QueryAsync(ctx => ctx.ServiceProviders.AnyAsync(p => p.ClientIdOnAggregator == pair.ClientId && p.AggregatorClientSecretHash == pair.ClientSecretHash)))
					return;
			}

			throw new UnifiedException(OAuth2Error.UnauthorizedClient, "Client Id or Client Secret is invalid.");
		}
	}
}