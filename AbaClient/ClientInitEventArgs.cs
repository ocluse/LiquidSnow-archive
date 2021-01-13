using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client
{
    public class ClientInitEventArgs : EventArgs
    {
        public string ClientID { get; set; }
        public string BaseAddress { get; set; }
        public string Version { get; set; }
        public IContext Context { get; set; }

        protected virtual void Configure(IConfiguration config)
        {
            var abaSection = config.GetSection("AbaClient");

            ClientID = abaSection.GetSection("ClientID").Value;
            BaseAddress = abaSection.GetSection("BaseAddress").Value;
            Version = abaSection.GetSection("Version").Value;
        }

    }

    public class MsalClientInitEventArgs : ClientInitEventArgs
    {
        public List<string> ApiScopes { get; set; }
        public string AadInstance { get; set; }
        public string PolicySUSI { get; set; }
        public string Tenant { get; set; }

        protected override void Configure(IConfiguration config)
        {
            base.Configure(config);
            var abaSection = config.GetSection("AbaClient");
            AadInstance = abaSection.GetSection("AadInstance").Value;
            PolicySUSI = abaSection.GetSection("PolicySUSI").Value;
            Tenant = abaSection.GetSection("Tenant").Value;
        }
    }

    public class ArethaClientInitEventArgs : MsalClientInitEventArgs
    {
        

        protected override void Configure(IConfiguration config)
        {
            var abaSection = config.GetSection("AbaClient");
            AadInstance = abaSection.GetSection("ArethaBaseAddress").Value;
            base.Configure(config);
        }
    }

    
}
