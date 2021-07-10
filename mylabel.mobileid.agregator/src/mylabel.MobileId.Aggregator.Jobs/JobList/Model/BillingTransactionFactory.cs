using System;
using System.Linq;
using mylabel.MobileId.Aggregator.Db;

namespace mylabel.MobileId.Aggregator.Jobs.JobList.Model
{
	public static class BillingTransactionFactory
	{
		static string FeatureCode(string scopes, string @operator, string acrValue, BillingFrom billingFrom)
		{
			string scopeForCode = ExtractScopeForCode(scopes, billingFrom);
			return CreateFeatureCode(scopeForCode, @operator, acrValue, billingFrom);
		}

		static string ExtractScopeForCode(string scopes, BillingFrom billingFrom)
		{
			string[] scopesArr = scopes.Split(" ");

			if (billingFrom == BillingFrom.Authorization)
			{
				if (scopesArr.Contains("mc_authn") && scopesArr.Length <= 2)
					return "mc_authn";
				return string.Empty;
			}
			else 
			{
				if (scopesArr.Contains("mc_identity_basic"))
					return "mc_identity_basic";
				else if (scopesArr.Contains("mc_identity_basic_address"))
					return "mc_identity_basic_address";
				else if (scopesArr.Contains("mc_identity_full"))
					return "mc_identity_full";
				else if (scopesArr.Contains("mc_identity_nationalid"))
					return "mc_identity_nationalid";
				//TODO: log scopes for analysis !
				return string.Empty;				
			}
		}

		static string CreateFeatureCode(string scopeForCode, string @operator, string acrValue, BillingFrom billingFrom)
		{
			if (billingFrom == BillingFrom.Authorization)
			{
				switch (scopeForCode + "," + @operator + "," + acrValue)
				{					
					case "mc_authn,tele2,3": return "";
				}
			}
			switch (scopeForCode + "," + @operator)
			{	
				case "mc_identity_nationalid,tele2": return "";
			}
			return string.Empty;
		}

		public static BillingTransaction? CreateNew(long subscriber_no, string? call_to_tn_sgsn, string? calling_no_ggsn, string scopes, string @operator, string acrValue, BillingFrom billingFrom, long authorizationId, string siOrDi)
		{
			string at_feature_code = FeatureCode(scopes, @operator, acrValue, billingFrom);

			if (!string.IsNullOrEmpty(at_feature_code))
				return new BillingTransaction()
				{
					subscriber_no = subscriber_no.ToString(),
					call_to_tn_sgsn = call_to_tn_sgsn ?? "",
					Calling_no_ggsn = calling_no_ggsn ?? "",
					at_feature_code = at_feature_code,
					AuthorizationId = authorizationId,
					SiOrDi = siOrDi,

					record_type = "01",
					channel_seizure_date_time = DateTimeOffset.Now,
					message_switch_id = "",
					call_action_code = "0",
					guide_by = "P",
					data_volume = 1,
					call_source = "A",
					basic_service_code = "MS",
					basic_service_type = "X",
					uom = "EV",
					technology = "G",
					call_destination = "",
				};
			return null;
		}
	}

	public enum BillingFrom
	{
		Authorization, PremiumInfo
	}
}
