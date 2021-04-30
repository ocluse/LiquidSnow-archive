using System;
using System.Text;
using Thismaker.Mercury;

namespace Test.Mercury.ServerTest
{
    class ServerApp
    {
        static Server Server { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Server = new Server
            {
                Address = "192.168.100.7",
                ListenPort = 32403
            };

            Server.ClientConnected += ClientConnected;
            Server.Received += Server_Received;
            Server.ClientClosed += ClientClosed;
            Server.Start();
            Console.WriteLine("Server Started");
            while (true)
            {

            }
        }

        private static void ClientClosed(ClientConnection client)
        {
            Console.WriteLine("Client disconnected without exceptions");
        }

        private static void Server_Received(ClientConnection arg1, byte[] arg2)
        {
            Console.WriteLine($"{arg1.ConnectionId}: {arg2.GetString<UTF8Encoding>()}");
        }

        private static void ClientConnected(ClientConnection obj)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Client {obj.ConnectionId} Connected");
            Console.ResetColor();
        }
    }
}