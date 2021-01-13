
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client.Clients
{
    public abstract class ArethaClient<T>:MsalClient<T> where T: ArethaClient<T>
    {
        public string ArethaBaseAddress { get; set; }

        public override void Configure(IConfiguration config)
        {
            var abaSection = config.GetSection("AbaClient");
            ArethaBaseAddress=abaSection.GetSection("ArethaBaseAddress").Value;
            base.Configure(config);
        }
    }
}
