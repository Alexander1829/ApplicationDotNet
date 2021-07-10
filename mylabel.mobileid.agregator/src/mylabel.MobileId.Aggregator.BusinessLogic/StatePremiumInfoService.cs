using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.Db;
using System.Linq;
using mylabel.MobileId.Aggregator.Common;

namespace mylabel.MobileId.Aggregator.BusinessLogic
{
	public class StatePremiumInfoService
	{
		public async Task<(
			PremiumInfoToken PremiumInfoToken,
			ServiceProvider ServiceProvider,
			string Scope,
			string ServingOperator,
			string AcrValues,
			string Msisdn,
			long AuthorizationId
			)>
			GetAuthStateByTokenAsync(string accessTokenFromAggregatorHash)
		{
			using var context = new AggregatorContext();
			var premiumInfoToken = await context!.PremiumInfoTokens!.FirstOrDefaultAsync(l => l.AccessTokenOnAggregatorHash == accessTokenFromAggregatorHash);
			if (premiumInfoToken == null)
				throw new UnifiedException(OAuth2Error.UnauthorizedClient, "No valid token found");

			string scope = string.Empty;
			string servingOperator = string.Empty;
			string acrValues = string.Empty;
			string msisdn = string.Empty;
			string clientIdOnAggregator = string.Empty;
			long authorizationId = 0;

			if (premiumInfoToken.AfterSiOrDi == "SI")
			{
				var authRequest = await context!.SIAuthorizationRequests!.FirstAsync(l => l.AccessTokenOnAggregatorHash == accessTokenFromAggregatorHash);
				clientIdOnAggregator = authRequest.ClientIdOnAggregator!;
				servingOperator = authRequest.ServingOperator!;
				scope = authRequest.Scope!;				
				acrValues = authRequest.AcrValues!;				
				msisdn = authRequest.Msisdn!;
				authorizationId = authRequest.Id;
			}
			else if (premiumInfoToken.AfterSiOrDi == "DI")
			{
				var authRequest = await context!.DIAuthorizationRequests!.FirstAsync(l => l.AccessTokenOnAggregatorHash == accessTokenFromAggregatorHash);
				clientIdOnAggregator = authRequest.ClientId!;
				servingOperator = authRequest.ServingOperator!;
				scope = authRequest.Scope!;				
				acrValues = authRequest.AcrValues!;				
				msisdn = authRequest.Msisdn ?? string.Empty;
				authorizationId = authRequest.Id;
			}
			var serviceProvider = await context!.ServiceProviders!.FirstOrDefaultAsync(l => l.ClientIdOnAggregator == clientIdOnAggregator);
			return (premiumInfoToken!, serviceProvider!, scope, servingOperator, acrValues, msisdn, authorizationId);
		}
	}
}
