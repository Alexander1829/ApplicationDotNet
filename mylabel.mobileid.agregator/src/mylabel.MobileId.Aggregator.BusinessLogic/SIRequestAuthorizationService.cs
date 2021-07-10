using System.Net.Http;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Cryptography;
using mylabel.MobileId.Aggregator.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using static System.Linq.Enumerable;
using static Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;

namespace mylabel.MobileId.Aggregator.BusinessLogic
{
	public class SIRequestAuthorizationService
	{
		HttpClient httpClient;
		CacheAccessor cacheAccessor;

		public SIRequestAuthorizationService(
			HttpClient httpClient,
			CacheAccessor cacheAccessor)
		{
			this.httpClient = httpClient;
			this.cacheAccessor = cacheAccessor;
		}

		public record Result(string DiscoUri, string DiscoClientId, string DiscoSecret, string DiscoRedirectUri);
		public async Task<Result> AuthorizeAndGetDiscoDetailsAsync(string clientId, string request)
		{
			var serviceProvider = await AggregatorContext.QueryAsync(ctx => ctx.ServiceProviders
				.Include(p => p.AllowedNotificationUris)
				.Include(p => p.Discoveries).ThenInclude(d => d.DiscoveryService)
				.FirstOrDefaultAsync(p => p.ClientIdOnAggregator == clientId));

			if (serviceProvider == null)
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, GetInvalidDescription(OpenIdConnectParameterNames.ClientId));

			var jwksString = serviceProvider.UseStoredJwksValue
				? serviceProvider.JwksValue
				: await cacheAccessor.GetOrCreateAsync(
					$"{GetType()}:{nameof(AuthorizeAndGetDiscoDetailsAsync)}:{serviceProvider.Id}",
					serviceProvider.JwksCachingInSeconds!.Value,
					() => httpClient.GetStringAsync(serviceProvider.JwksUri));

			if (!JwtSignatureValidator.Validate(request, jwksString!, out var claims))
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, "Token validation failed");

			claims!.TryGetValue(MobileConnectParameterNames.NotificationUri, out var value2);

			if (!serviceProvider.AllowedNotificationUris!.Any(u => u.Value == (claims!.TryGetValue(MobileConnectParameterNames.NotificationUri, out var value) ? (string?)value : null)))
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, GetInvalidDescription(MobileConnectParameterNames.Request));

			var disco = serviceProvider.Discoveries!.First(d => d.IsEnabled);

			return new(disco.DiscoveryService!.Uri!, disco.ClientIdOnDiscovery!, disco.ClientSecretOnDiscovery!, disco.RedirectUri!);
		}

		static string GetInvalidDescription(string parameterName) => $"Mandatory parameter {parameterName} is invalid";
	}
}