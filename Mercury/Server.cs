using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;


namespace Thismaker.Mercury
{
    public class Server
    {
        public void Start(int tcpPort)
        {
            var serverSocket = new TcpListener(tcpPort);
            var clientSocket = default(TcpClient);
        }
    }
}
