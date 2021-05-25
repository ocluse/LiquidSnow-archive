using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;
using MServer = Thismaker.Mercury.Server;

namespace Thismaker.Aba.Server.Mercury
{
    public abstract partial class MercuryServer
    {
        private readonly MServer _mServer;
        private readonly bool _requiresAuth;
        private readonly List<string> _defaultScopes;
        private event Action<string, string> ClientProvidedAuthentication;
        #region Properties

        public string Address { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// The interval in milliseconds to wait to send PING messages to the clients. 
        /// The default is 5000ms.
        /// </summary>
        public double PingInterval { get; set; } = 5000;

        /// <summary>
        /// The interval in milliseconds to wait for a ping response from the client.
        /// Faliure to which the connection times out. The default is 2000ms
        /// </summary>
        public double TimeoutMilliseconds { get; set; } = 2000;

        /// <summary>
        /// If provided, the server authenticates itself using the certificate.
        /// This allows for secure TLS connection
        /// </summary>
        public X509Certificate Certificate { get; set; } = null;

        #endregion

        #region Initialization
        protected MercuryServer()
        {
            var requireAuthAtt = GetType().GetCustomAttribute<RequireTokenValidationAttribute>();
            _requiresAuth = requireAuthAtt != null;
            
            if (_requiresAuth)
            {
                _defaultScopes = requireAuthAtt.Scopes;
            }

            //Initialize the Mercury Server instance
            _mServer = new MServer();
            _groups = new Dictionary<string, Group>();
            _users = new Dictionary<string, MercuryUser>();
            //Subscribe to events
            _mServer.ClientConnected += OnConnectedInternal;
            _mServer.ClientClosed += OnDisconnectedInternal;
            _mServer.Received += OnReceived;
        }

        /// <summary>
        /// Starts the Server connection, allowing it to receive client connections
        /// </summary>
        public void Start()
        {
            _mServer.Address = Address;
            _mServer.Port = Port;
            _mServer.TimeoutMiliseconds = TimeoutMilliseconds;
            _mServer.PingInterval = PingInterval;

            _mServer.Certificate = Certificate;
            _mServer.Start();
        }

        /// <summary>
        /// Stops the server, disconnecting all clients.
        /// </summary>
        public async Task StopAsync()
        {
            await _mServer.StopAsync().ConfigureAwait(false);
        }

        private async void OnConnectedInternal(string connectionId)
        {
            MercuryUser user;
            
            if (_requiresAuth)
            {
                var _tcsAuthenticate = new TaskCompletionSource<string>();

                void OnCLientProvidedAuthentication(string connId, string accessToken)
                {
                    if (connId == connectionId)
                    {
                        ClientProvidedAuthentication -= OnCLientProvidedAuthentication;
                        _tcsAuthenticate.SetResult(connId);
                    }
                }

                ClientProvidedAuthentication += OnCLientProvidedAuthentication;
                await _mServer.SendAsync(connectionId, Globals.AuthSelf).ConfigureAwait(false);

                var token = await _tcsAuthenticate.Task;

                var principal = await ValidateAccessToken(token, _defaultScopes).ConfigureAwait(false);

                if (principal != null)
                {
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    AddUserConnectionId(connectionId, userId);
                    user = User(userId);
                }
                else
                {
                    //User is not authorized, closed their connection:
                    await DisconnectClientAsync(connectionId, "AUTH_FAILED").ConfigureAwait(false);
                    return;
                }
            }
            else
            {
                user = new MercuryUser(null, this);
                user.AddConnectionId(connectionId);
            }


            await OnConnectedAsync(user, connectionId).ConfigureAwait(false);
            
        }

        private async void OnDisconnectedInternal(string connectionId, Exception ex)
        {
            MercuryUser user = UserWithConnectionId(connectionId);

            await OnDisconnectedAsync(user, connectionId, ex);
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

        /// <summary>
        /// This method is called to validate an access token that is provided by the user. 
        /// </summary>
        /// <param name="accessToken">The token string</param>
        /// <param name="scopes">The scopes, if neccessary</param>
        /// <returns></returns>
        protected abstract Task<ClaimsPrincipal> ValidateAccessToken(string accessToken, List<string> scopes);

        #endregion

        #region Virtuals

        /// <summary>
        /// Called whenever a client disconnects.
        /// </summary>
        /// <param name="connectionId">The unique Id of the new client</param>
        /// <param name="ex">The cause for disconnect, if graceful, returns null</param>
        protected virtual Task OnDisconnectedAsync(MercuryUser user, string connectionId, Exception ex)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called whenever a new client connects
        /// </summary>
        /// <param name="connectionId">The unique id of the connected client</param>
        protected virtual Task OnConnectedAsync(MercuryUser user, string connectionId)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        private void OnReceived(string connectionId, byte[] data)
        {
            //Check if it's ping:
            var json = data.GetString<UTF8Encoding>();
            var payload = Deserialize<RPCPayload>(json);
            PayloadReceivedAsync(connectionId, payload);
        }

        #endregion
    }
}
