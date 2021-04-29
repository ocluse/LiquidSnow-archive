using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Mercury
{
    public class ClientConnection
    {
        public int ReceiveBufferSize { get; set; }

        public TcpClient TcpClient { get; set; }

        public string ConnectionId { get; set; }

        public Socket Socket
        {
            get { return TcpClient.Client; }
        }

        public event Action<ClientConnection, byte[]> OnReceive;

        readonly CancellationTokenSource ctsMain;

        public ClientConnection()
        {
            ConnectionId = Horus.Horus.GenerateID();
            ctsMain = new CancellationTokenSource();
        }

        public async void Start()
        {
            await Receive(ctsMain.Token);
        }

        private async Task Receive(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (Socket.Available == 0) continue;
                    var bufferSize = Socket.Available;

                    var buffer = new byte[bufferSize];

                    Socket.Receive(buffer);

                    OnReceive?.Invoke(this, buffer);
                }
            });
           
        }

        public Task Send(byte[] data)
        {
            try
            {
                Socket.Send(data);
                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
        }
    }
}
