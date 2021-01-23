using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TEnigma=Thismaker.Enigma.Enigma;

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
            ConnectionId = TEnigma.GenerateID();
            ctsMain = new CancellationTokenSource();
        }

        public void Start()
        {
            Receive(ctsMain.Token).Start();
        }

        private Task Receive(CancellationToken cancellationToken)
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

            return Task.CompletedTask;
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
