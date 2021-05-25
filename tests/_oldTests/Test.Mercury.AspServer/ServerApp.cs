using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Thismaker.Aba.Server.Mercury;
using Thismaker.Aba.Server.Mercury.Hosted;

namespace Test.Mercury.AspServer
{
    public class ServerApp : HostedMercuryServer
    {

        [Beamable]
        public async Task Send(IPrincipal principal, string arg1)
        {
            await BeamClientsAsync("Receive", arg1);
        }

        protected override void OnDisconnected(string connectionId, Exception ex)
        {
            Console.WriteLine("client disconnected");
        }

    }
}
