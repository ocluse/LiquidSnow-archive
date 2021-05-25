using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Aba.Server.Mercury.Hosted
{
    public class HostedMercuryManager: IHostedService
    {
        private readonly HostedManagerOptions _options;
        private readonly IServiceProvider _provider;
        private readonly List<HostedMercuryServer> _servers;
        private readonly string _hostAddress;

        private Dictionary<string, MercuryUser> _users;

        public HostedMercuryManager(HostedManagerOptions options, IServiceProvider provider) 
        { 
            _options = options;
            _provider = provider;
            _servers = new List<HostedMercuryServer>();
            _hostAddress = options.HostAddress;
        }

        #region Hosted Services

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //Create the servers
            foreach (var option in _options.ServerOptions)
            {
                var constructors = option.ServiceType.GetConstructors();

                var constructor = constructors.First();

                foreach(var con in constructors)
                {
                    if (con.GetCustomAttributes(false).Any(x => x.GetType() == typeof(BeamerConstructorAttribute)))
                    {
                        constructor = con;
                        break;
                    }
                }

                var paramInfos = constructor.GetParameters();

                var parameters = new List<object>();

                if (paramInfos.Length > 0)
                {
                    foreach(var paramInfo in paramInfos)
                    {
                        parameters.Add(_provider.GetService(paramInfo.ParameterType));
                    }
                }

                var server = (HostedMercuryServer)Activator.CreateInstance(option.ServiceType, parameters.ToArray());

                server.Address = string.IsNullOrEmpty(option.Address) ?
                    "localhost" : option.Address;
                server.Port = option.Port;
                server.SetManager(this);
                _servers.Add(server);

                await server.StartAsync(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(var server in _servers)
            {
                await server.StopAsync(cancellationToken);
            }
        }

        #endregion

        #region Hosted Specifics
        public async Task<ClaimsPrincipal> ValidateAccessToken(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization
                    = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                httpClient.BaseAddress = new Uri(_hostAddress);
                try
                {
                    var response = await httpClient.GetAsync(AuthenticatorController.Route);

                    if (response.IsSuccessStatusCode)
                    {
                        return Deserialize<ClaimsPrincipal>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        internal MercuryUser GetUser(string userId, HostedMercuryServer server)
        {
            if (_users == null) return null;

            if (!_users.ContainsKey(userId)) return null;

            var user = new MercuryUser(_users[userId]);

            user.SetServer(server);

            return user;
        }

        internal virtual MercuryUser GetUserWithConnectionId(string connectionId, HostedMercuryServer server)
        {
            if (_users == null) return null;

            foreach (var user in _users)
            {
                if (user.Value.HasConnectionId(connectionId))
                {
                    var result = new MercuryUser(user.Value);
                    result.SetServer(server);
                    return result;
                }
            }

            return null;
        }

        internal void AddUserConnectionId(string connectionId, string userId)
        {
            MercuryUser user;

            if (_users == null) _users = new Dictionary<string, MercuryUser>();

            if (_users.ContainsKey(userId))
            {
                user = _users[userId];
            }
            else
            {
                user = new MercuryUser(userId, null);
            }

            user.AddConnectionId(connectionId);
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Called by the HTTP POST helper to create a string content out of a provided object. Override to customize the behaviour.
        /// By default, the string content is encoded using UTF8 encoding with a mediatype of "application/json"
        /// </summary>
        protected virtual StringContent GetStringContent<T>(T o)
        {
            return new StringContent(Serialize(o), System.Text.Encoding.UTF8, "application/json");
        }

        private string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize<T>(obj);
        }

        private T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        #endregion


    }
}
