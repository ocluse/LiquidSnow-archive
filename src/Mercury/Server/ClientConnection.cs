using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
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

        public event Action<ClientConnection, byte[]> Received;

        public event Action<ClientConnection> Closed;

        readonly CancellationTokenSource ctsMain;

        public ClientConnection()
        {
            ConnectionId = Horus.Horus.GenerateID();
            ctsMain = new CancellationTokenSource();
        }

        public void Start()
        {
            Task.Run(Receive);
        }

        public void Close()
        {
            ctsMain.Cancel();
        }

        private void Receive()
        {
            while (true)
            {
                if (ctsMain.IsCancellationRequested)
                {
                    break;
                }

                if (Socket.Available == 0) continue;
                var bufferSize = Socket.Available;

                var buffer = new byte[bufferSize];

                Socket.Receive(buffer);

                Received?.Invoke(this, buffer);
            }

            Closed?.Invoke(this);
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
