using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Thismaker.Mercury
{
    public class Client
    {
        private TcpClient client;

        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }

        public event Action<byte[]> OnReceive;
        public event Action OnConnected;


        public async void Connect()
        {
            var ip = Dns.GetHostAddresses(ServerAddress).ElementAt(0);
            client = new TcpClient(ip.ToString(), ServerPort);
            OnConnected?.Invoke();
            await Listen();
        }

        public Task Send(byte[] data)
        {
            var ns = client.GetStream();
            ns.Write(data, 0, data.Length);
            return Task.CompletedTask;
        }

        private async Task Listen()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var bufferSize = client.Available;

                        if (bufferSize == 0) continue;

                        var ns = client.GetStream();
                        var buffer = new byte[bufferSize];
                        ns.Read(buffer, 0, bufferSize);
                        OnReceive?.Invoke(buffer);
                    }
                    catch
                    {
                        break;
                    }
                }
            });
            
        }
    }
}
