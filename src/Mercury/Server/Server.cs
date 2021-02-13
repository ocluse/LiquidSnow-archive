using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
namespace Thismaker.Mercury
{
    public class Server
    {

        public int ListenPort { get; set; }

        public string Address { get; set; }

        protected TcpListener listener;

        public List<ClientConnection> Clients { get; set; }

        public Socket clientSocket;

        public Server()
        {
            Clients = new List<ClientConnection>();
        }

        public void Start()
        {
            ServerRun().Start();
        }

        private Task ServerRun()
        {
            var ip = Dns.GetHostAddresses(Address);

            listener = new TcpListener(ip.First(), ListenPort);
            listener.Start();
            while (true)
            {
                try
                {
                    var tcpClient = listener.AcceptTcpClient();
                    var client = new ClientConnection
                    {
                        TcpClient = tcpClient
                    };
                    client.Start();

                    Clients.Add(client);
                }
                catch
                {
                    break;
                }
            }
            return Task.CompletedTask;
        }
    }
}
