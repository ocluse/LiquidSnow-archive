using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Thismaker.Mercury
{
    public class Server
    {
        #region Private Fields
        private bool _connected;
        private readonly Dictionary<string, ClientConnection> _clients;
        private TcpListener _listener;
        #endregion

        #region Events

        /// <summary>
        /// Fired whenever a new client connects with the unique ID of that client
        /// </summary>
        public event Action<string> ClientConnected;

        /// <summary>
        /// Fired whenever a client diconnects. Exception will be null if closer was graceful
        /// </summary>
        public event Action<string, Exception> ClientClosed;

        /// <summary>
        /// Fired whenever a client has sent information, with a string of the client's ID
        /// </summary>
        public event Action<string, byte[]> Received;

        #endregion

        #region Public Methods

        /// <summary>
        /// The amount of time for a client to be considered disconnected if no response is heard.
        /// This is only useful when a <see cref="PingInterval"/> value is provided.
        /// </summary>
        public double TimeoutMiliseconds { get; set; }

        /// <summary>
        /// If provided, the length of time to periodically send ping messages in milliseconds.
        /// </summary>
        public double? PingInterval { get; set; }

        /// <summary>
        /// The port that the server listens for client connections
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The address that the server runs on
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// When provided, the server-client connection will be authenticated, 
        /// allowing for secure TLS communications
        /// </summary>
        public X509Certificate Certificate { get; set; }

        #endregion

        public Server()
        {
            _clients = new Dictionary<string, ClientConnection>();
        }

        public void Start()
        {
            _connected = true;
            Task.Run(RunInternal);
        }

        public void Stop()
        {
            _connected = false;

            while (_clients.Count > 0)
            {
                _clients.First().Value.Disconnect();
            }
        }

        public bool HasClient(string connectionId)
        {
            return _clients.ContainsKey(connectionId);
        }

        public async Task SendAsync(string connectionId, byte[] data)
        {
            await _clients[connectionId].SendAsync(data);
        }

        public async Task SendAllAsync(byte[] data)
        {
            foreach(var client in _clients)
            {
                await client.Value.SendAsync(data);
            }
        }

        private async Task RunInternal()
        {
            var ip = Dns.GetHostAddresses(Address);

            _listener = new TcpListener(ip.First(), Port);
            _listener.Start();
            
            while (_connected)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                var client = new ClientConnection(tcpClient, this);
                client.Received += OnClientReceived;
                client.Closed += OnClientClosed;
                _clients.Add(client.ConnectionId, client);
                ClientConnected?.Invoke(client.ConnectionId);
            }
        }

        private void OnClientClosed(ClientConnection client, Exception ex)
        {
            client.Closed -= OnClientClosed;
            client.Received -= OnClientReceived;
            _clients.Remove(client.ConnectionId);
            ClientClosed?.Invoke(client.ConnectionId, ex);
        }

        private void OnClientReceived(ClientConnection client, byte[] data)
        {
            Received?.Invoke(client.ConnectionId, data);
        }
    }
}
