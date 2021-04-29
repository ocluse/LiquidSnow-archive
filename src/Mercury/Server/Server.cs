using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System;

namespace Thismaker.Mercury
{
    public class Server
    {

        public int ListenPort { get; set; }

        public string Address { get; set; }

        protected TcpListener listener;

        public List<ClientConnection> Clients { get; set; }

        public event Action<ClientConnection> ClientConnected;

        public event Action<ClientConnection, byte[]> Received;

        public Socket clientSocket;

        public Server()
        {
            Clients = new List<ClientConnection>();
        }

        public async void Start()
        {
            await ServerRun();
        }

        private async Task ServerRun()
        {
            var ip = Dns.GetHostAddresses(Address);

            listener = new TcpListener(ip.First(), ListenPort);
            listener.Start();
            while (true)
            {
                try
                {
                    var tcpClient = await listener.AcceptTcpClientAsync();
                    var client = new ClientConnection
                    {
                        TcpClient = tcpClient
                    };
                    client.OnReceive += Client_OnReceive;
                    client.Start();

                    Clients.Add(client);
                    ClientConnected?.Invoke(client);
                }
                catch
                {
                    break;
                }
            }
        }

        private void Client_OnReceive(ClientConnection arg1, byte[] arg2)
        {
            Received?.Invoke(arg1, arg2);
        }
    }
}
