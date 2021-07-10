using mylabel.MobileId.Aggregator.Db;

namespace mylabel.MobileId.Aggregator.Admin.Model.Helper
{
	public static class Mapper
	{
		public static ServiceProvider CreateNew(FormServiceProvider formSP)
		{
			var result = new ServiceProvider
			{
				ClientIdOnAggregator = formSP.ValidClientIdOnAggregator,
				Name = formSP.ValidClientName,
				AggregatorClientSecretHash = formSP.ValidAggregatorClientSecretHash,

				JwksUri = formSP.ServiceProvider.JwksUri,
				JwksValue = formSP.ServiceProvider.JwksValue,
				UseStoredJwksValue = formSP.ServiceProvider.UseStoredJwksValue,
				JwksCachingInSeconds = formSP.ServiceProvider.JwksCachingInSeconds,
				IsInactive = formSP.ServiceProvider.IsInactive,
				IsPremiumInfoSigned = formSP.ServiceProvider.IsPremiumInfoSigned,
				CreatedAt = formSP.ServiceProvider.CreatedAt,
				BillingCtn = formSP.ValidBillingCtn,
				Id = formSP.ServiceProvider.Id,
				AllowedIPAddresses = string.IsNullOrWhiteSpace(formSP.AllowedIPAddresses) ? null : formSP.AllowedIPAddresses.Trim()
			};
			return result;
		}
	}
}