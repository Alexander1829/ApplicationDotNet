using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mylabel.MobileId.Aggregator.Integrations.Discovery.Model
{
    public class ClientToDiscoveryAuthSettings
    {
        private string clientId;
        private string clientSecret;
        public ClientToDiscoveryAuthSettings(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }
        public string DiscoveryAuthString => Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
    }
}
