using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;
using Thismaker.Aba.Server.Mercury;
using Thismaker.Mercury;

namespace Test.Mercury.ServerTest
{
    class Program
    {
        static ServerApp App;

        public static void Main()
        {
            App = new ServerApp
            {
                Address = "localhost",
                Port = 32403
            };
            var cert = new X509Certificate("certificate.pfx", "AceIK58$");
            App.Start(null);
            while (true) { }
        }
    }

    class ServerApp : MercuryServer
    {
        public ServerApp()
        {
            
        }

        protected override T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        protected override string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        [Beamable]
        public async Task Send(string arg1)
        {
            await BeamClientsAsync("Receive", arg1);
        }

        protected override void OnDisconnected(string connectionId, Exception ex)
        {
            Console.WriteLine("client disconnected");
        }

        protected override string Serialize(object obj, Type type)
        {
            return JsonSerializer.Serialize(obj, type);
        }

        protected override object Deserialize(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type);
        }

        protected override IPrincipal ValidateAccessToken(string accessToken, List<string> scopes)
        {
            throw new NotImplementedException();
        }
    }
}