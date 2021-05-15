using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thismaker.Aba.Client.Mercury;
using Thismaker.Aba.Common.Mercury;
using Thismaker.Mercury;

namespace Test.Mercury.ClientTest
{
    class Program
    {
        static ClientApp App;

        public static async Task Main()
        {
            App = new ClientApp
            {
                BaseAddress = "https://localhost",
                Port = 32403
            };
            App.MakeApp();
            await App.Connect();
            await App.Start(null);
            
            Console.WriteLine("Connected");
            while (true)
            {
                var line = Console.ReadLine();

                App.SendMessage(line);
                
            }
        }
    }

    class ClientApp : MercuryClient<ClientApp>
    {
        public override Task<string> GetCloudVersion()
        {
            throw new NotImplementedException();
        }

        public override Task Start(IProgress<string> progress)
        {
            Subscribe<string>(Receive);
            return Task.CompletedTask;
        }

        public void SendMessage(string message)
        {
            Beam("Send", message);
        }

        public void Receive(string arg1)
        {
            Console.WriteLine(arg1);
        }

        protected override T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        protected override object Deserialize(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type);
        }

        protected override string Serialize(object obj, Type type)
        {
            return JsonSerializer.Serialize(obj, type);
        }

        protected override string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize<T>(obj);
        }
    }
}
