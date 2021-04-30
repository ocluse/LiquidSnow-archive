using System;
using System.Text;
using Thismaker.Mercury;

namespace Test.Mercury.ClientTest
{
    class ClientApp
    {
        static Client Client { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Client!");
            Client = new Client
            {
                ServerAddress = "192.168.100.7",
                ServerPort = 32403
            };

            Client.OnConnected += Client_OnConnected;
            Client.OnReceive += OnClientReceived;
            _ = Client.ConnectAsync();

            while (true)
            {
                string message = Console.ReadLine();

                Client.Send(message.GetBytes<UTF8Encoding>());
            }
        }

        private static void OnClientReceived(byte[] obj)
        {
            Console.WriteLine(obj.GetString<UTF8Encoding>());
        }

        private static void Client_OnConnected()
        {
            Console.WriteLine("Connected to Server");
        }
    }
}
