using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using mylabel.MobileId.Aggregator.Common;
using mylabel.MobileId.Aggregator.Integrations.Idgw;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace mylabel.MobileId.Aggregator.BusinessLogic
{
	public class IdgwConnectorManager
	{
		IOptions<Settings> settings;
		Dictionary<ServingOperator, IdgwConnector> IdgwConnectors;
		static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
		ILogger<IdgwConnector> idgwConnectorLogger;

		public IdgwConnectorManager(IOptions<Settings> settings, ILogger<IdgwConnector> idgwConnectorLogger)
		{
			this.settings = settings;
			IdgwConnectors = new Dictionary<ServingOperator, IdgwConnector>();
			this.idgwConnectorLogger = idgwConnectorLogger;
		}

		public IdgwConnector this[ServingOperator i] {
			get => IdgwConnectors[i];
		}

		public async Task<bool> TryAddConnectorAsync(string operatorAsString, string oidcConfigUri)
		{
			ServingOperator sOperator = GetServingOperatorByString(operatorAsString);
			if (IdgwConnectors.ContainsKey(sOperator))
				return false;
			await semaphoreSlim.WaitAsync();
			if (!IdgwConnectors.ContainsKey(sOperator))
			{
				var idConfig = await GetOidcConfigurationAsync(oidcConfigUri);
				var idgwConnector = new IdgwConnector(new HttpClient(), settings, idConfig, idgwConnectorLogger);
				if (sOperator == ServingOperator.Mts)
					idgwConnector.OpenIdConfig.PremiumInfoEndpoint = settings.Value.PremiumInfoUri!.Mts;
				IdgwConnectors.Add(sOperator, idgwConnector);
				semaphoreSlim.Release();
				return true;
			}
			semaphoreSlim.Release();
			return false;
		}

		public ServingOperator GetServingOperatorByString(string operatorAsString)
		{
			return (ServingOperator)Enum.Parse(typeof(ServingOperator), operatorAsString, true);
		}

		private async Task<MobileConnectConfiguration> GetOidcConfigurationAsync(string openidConfigUri)
		{
			var httpClient = new HttpClient();
			var idgwResponse = await httpClient.GetAsync(openidConfigUri);
			var responseBody = await idgwResponse.Content.ReadAsStringAsync();

			return new MobileConnectConfiguration(responseBody)!;
		}
	}
}