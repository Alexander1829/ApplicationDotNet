using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.BusinessLogic;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Cryptography;
using mylabel.MobileId.Aggregator.Db;
using mylabel.MobileId.Aggregator.Integrations.Idgw;
using mylabel.MobileId.Aggregator.Jobs.JobList.Model;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static System.Collections.Immutable.ImmutableArray;
using static System.Linq.Enumerable;

namespace mylabel.MobileId.Aggregator.Api.Controllers
{
	[ApiController]
	public class PremiumInfoController : ControllerBase
	{
		IdgwConnectorManager idgwConnectorManager;
		StatePremiumInfoService authorizationStateService;
		PremiumInfoValidationService premiumInfoValidationService;
		Settings settings;
		ILogger<PremiumInfoController> logger;

		public PremiumInfoController(
			IdgwConnectorManager idgwConnectorManager,
			StatePremiumInfoService authorizationStateService,
			PremiumInfoValidationService premiumInfoValidationService,
			IOptions<Settings> settings,
			ILogger<PremiumInfoController> logger)
		{
			this.idgwConnectorManager = idgwConnectorManager;
			this.authorizationStateService = authorizationStateService;
			this.premiumInfoValidationService = premiumInfoValidationService;
			this.settings = settings.Value;
			this.logger = logger;
		}

		[Route("premiuminfo")]
		[HttpGet, HttpPost]
		public async Task<object> PremiumInfo()
		{
			var accessTokenOnAggregator = premiumInfoValidationService.CheckAndGetAccessTokenOnAggregator(Request);
			var atHash = HelperHash.AtHashString(accessTokenOnAggregator);

			var premiumInfoTokenRequest = await authorizationStateService.GetAuthStateByTokenAsync(atHash);

			if (!string.IsNullOrWhiteSpace(premiumInfoTokenRequest.ServiceProvider.AllowedIPAddresses))
			{
				var v4IP = HttpContext.Connection.RemoteIpAddress!.MapToIPv4();
				if (!IsIPAllowed(premiumInfoTokenRequest.ServiceProvider.AllowedIPAddresses, v4IP))
					throw new UnifiedException(OAuth2Error.AccessDenied, $"IP address {v4IP} not allowed.");
			}

			var servingOperator = idgwConnectorManager.GetServingOperatorByString(premiumInfoTokenRequest.ServingOperator);
			var idgwConnector = idgwConnectorManager[servingOperator];

			var premiumInfoResponse = await idgwConnector.PremiumInfoAsync(premiumInfoTokenRequest.PremiumInfoToken.AccessTokenOnIdgw!, await ReadRequestBody());

			var result = premiumInfoTokenRequest.ServiceProvider.IsPremiumInfoSigned ?
				premiumInfoResponse ://TODO unable to create payload from one string. SignedJwtCreator.Create(premiumInfoResponse, settings.PrivateKey!, settings.Kid) :
				premiumInfoResponse;

			var transaction = BillingTransactionFactory.CreateNew(
				premiumInfoTokenRequest.ServiceProvider.BillingCtn!,
				premiumInfoTokenRequest!.Msisdn,
				premiumInfoTokenRequest!.Msisdn,
				premiumInfoTokenRequest!.Scope,
				premiumInfoTokenRequest!.ServingOperator,
				premiumInfoTokenRequest!.AcrValues,
				BillingFrom.PremiumInfo,
				premiumInfoTokenRequest!.AuthorizationId,
				premiumInfoTokenRequest!.PremiumInfoToken.AfterSiOrDi!);
			if (transaction != null)
				await AggregatorContext.SaveAsync(ctx => ctx.BillingTransactions!.AddAsync(transaction));
			else
				logger.LogError("Transaction not billed");

			return result;
		}

		static bool IsIPAllowed(string allowedAddresses, IPAddress remoteIP)
		{
			foreach (var item in allowedAddresses.Replace(" ", string.Empty).Split(","))
			{
				if (item.Contains("/"))
				{
					var values = item.Split("/");
					IPNetwork network = new(IPAddress.Parse(values.First()), int.Parse(values.Last()));
					if (network.Contains(remoteIP))
						return true;
				}
				else if (item.Contains("-"))
				{
					var borders = item.Split("-").Select(s => IPToInteger(IPAddress.Parse(s))).ToImmutableArray();
					var integerRemote = IPToInteger(remoteIP);
					if (integerRemote >= borders.First() && integerRemote <= borders.Last())
						return true;
				}
				else
				{
					if (IPAddress.Parse(item).Equals(remoteIP))
						return true;
				}
			}

			return false;
		}

		static uint IPToInteger(IPAddress address)
		{
			var bytes = address.GetAddressBytes();
			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);
			return BitConverter.ToUInt32(bytes);
		}

		async Task<PremiumInfoBody?> ReadRequestBody()
		{
			if (Request.Method == WebRequestMethods.Http.Post)
			{
				using var streamReader = new StreamReader(Request.Body);
				var stringBody = await streamReader.ReadToEndAsync();
				try
				{
					if (!string.IsNullOrEmpty(stringBody))
						return JsonSerializer.Deserialize<PremiumInfoBody>(stringBody);
				}
				catch
				{
					throw new UnifiedException(OAuth2Error.InvalidRequest, "Unsupported Media Type");
				}
			}
			return null;
		}
	}
}