using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mylabel.MobileId.Aggregator.Integrations.Discovery.Model
{
    public class ProxySettings
    {
        public ProxySettings(string address, string? username = null, string? password = null, string? domain = null) 
        {
            this.Address = address;            
            this.Username = username;
            this.Password = password;
            this.Domain = domain;
        }
        public string Address { get; }
        public string Username { get; }
        public string Password { get; }
        public string Domain { get; }
    }
}
