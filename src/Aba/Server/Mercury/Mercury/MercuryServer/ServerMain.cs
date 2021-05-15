using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;
using Thismaker.Mercury;
using MServer = Thismaker.Mercury.Server;

namespace Thismaker.Aba.Server.Mercury
{
    public abstract partial class MercuryServer
    {
        private readonly MServer _mServer;

        #region Properties

        public string Address { get; set; }

        public int Port { get; set; }

        public double PingInterval { get; set; } = 5000;

        public double TimeoutMilliseconds { get; set; } = 2000;

        #endregion

        #region Initialization
        protected MercuryServer()
        {
            //Initialize the Mercury Server instance
            _mServer = new MServer();

            //Subscribe to events
            _mServer.ClientConnected += OnConnected;
            _mServer.ClientClosed += OnDisconnected;
            _mServer.Received += OnReceived;
        }

        /// <summary>
        /// Starts the Server connection, allowing it to receive client connections
        /// </summary>
        public void Start(X509Certificate certificate=null)
        {
            _mServer.Address = Address;
            _mServer.Port = Port;
            _mServer.TimeoutMiliseconds = TimeoutMilliseconds;
            _mServer.PingInterval = PingInterval;

            _mServer.Certificate = certificate;
            _mServer.Start();
        }
        #endregion

        #region Abstracts
        
        /// <summary>
        /// This method is used to serialize the <see cref="RPCPayload"/> for transportation.
        /// Override to customize payload serialization
        /// </summary>
        protected abstract T Deserialize<T>(string json);

        /// <summary>
        /// This method is used to deserialize a received payload. 
        /// Override to customize deserialization.
        /// </summary>
        protected abstract string Serialize<T>(T obj);

        protected abstract string Serialize(object obj, Type type);

        protected abstract object Deserialize(string json, Type type);

        protected abstract Task<IPrincipal> ValidateAccessToken(string accessToken, List<string> scopes);

        #endregion

        #region Virtuals

        protected virtual void OnDisconnected(string connectionId, Exception ex)
        {

        }

        protected virtual void OnConnected(string connectionId)
        {

        }

        #endregion

        #region Private Methods

        private void OnReceived(string connectionId, byte[] data)
        {
            //Check if it's ping:
            var json = data.GetString<UTF8Encoding>();
            var payload = Deserialize<RPCPayload>(json);
            PayloadReceived(payload);
        }

        #endregion
    }
}
