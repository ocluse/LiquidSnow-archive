using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;

namespace Thismaker.Aba.Server.Mercury
{
    public partial class MercuryServer : IBeamer
    {
        private class CacheItem
        {
            public bool Authorize { get; set; }
            public bool InjectPrincipal { get; set; }
            public MethodInfo MethodInfo { get; set; }
            public List<Type> Types { get; set; }
            public List<string> Scopes { get; set; }
        }

        private Dictionary<string, CacheItem> _methodCache;

        #region Payload

        private void ConstructMethodCache()
        {
            _methodCache = new Dictionary<string, CacheItem>();

            MethodInfo[] methods = GetType().GetMethods(BindingFlags.Instance);

            foreach (MethodInfo method in methods)
            {
                BeamableAttribute beamable = method.GetCustomAttribute<BeamableAttribute>();
                if (beamable == null)
                {
                    continue;
                }

                string name = string.IsNullOrEmpty(beamable.MethodName) ?
                    method.Name : beamable.MethodName;

                List<Type> types = new List<Type>();
                ParameterInfo[] pInfos = method.GetParameters();

                foreach (ParameterInfo pInfo in pInfos)
                {
                    types.Add(pInfo.ParameterType);
                }

                CacheItem cache = new CacheItem
                {
                    MethodInfo = method,
                    Types = types,
                    Authorize = _requiresAuth || beamable.Authorized
                };

                if (cache.Types[0] == typeof(MercuryPrincipal))
                {
                    cache.InjectPrincipal = true;
                    cache.Authorize = true;
                }

                if (beamable.Scopes.Count > 0)
                {
                    cache.Scopes = new List<string>(beamable.Scopes);
                    cache.Authorize = true;
                }

                _methodCache.Add(name, cache);
            }
        }

        private async Task SendPayloadAsync(string connectionId, RPCPayload payload)
        {
            await _mServer.SendAsync(connectionId, Serialize(payload).GetBytes<UTF8Encoding>())
                .ConfigureAwait(false);
        }

        private async void PayloadReceivedAsync(string connectionId, RPCPayload payload)
        {
            //check if the method is an authentication payload
            if (payload.MethodName == Globals.AuthResponsePayload)
            {
                ClientProvidedAuthentication?.Invoke(connectionId, payload.AccessToken);
                return;
            }

            if (_methodCache == null)
            {
                ConstructMethodCache();
            }

            if (!_methodCache.ContainsKey(payload.MethodName))
            {
                return;
            }

            //if we require auth send
            CacheItem item = _methodCache[payload.MethodName];

            List<object> parameters = new List<object>();

            //first, validate the access token:
            if (item.Authorize)
            {
                ClaimsPrincipal principal = await ValidateAccessToken(payload.AccessToken, item.Scopes).ConfigureAwait(false);

                if (principal == null)
                {
                    return;
                }

                string userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                //If we require authentication, ensure that is done first:
                if (_requiresAuth && User(userId) == null)
                {
                    await DisconnectClientAsync(connectionId, "NOT_AUTH").ConfigureAwait(false);
                    return;
                }

                //Add the user if not exists, only in non-auth mode.
                if (!string.IsNullOrEmpty(userId) && !_requiresAuth)
                {
                    AddUserConnectionId(connectionId, userId);
                }

                //Inject the principal if so required
                if (item.InjectPrincipal)
                {
                    MercuryPrincipal mPrincipal = new MercuryPrincipal(connectionId, this, principal);
                    parameters.Add(mPrincipal);
                }
            }

            if (payload.Parameters.Count > 0)
            {
                for (int i = 0; i < payload.Parameters.Count; i++)
                {
                    parameters.Add(Deserialize(payload.Parameters[i], item.Types[i]));
                }
            }

            _ = parameters.Count == 0 ? item.MethodInfo.Invoke(this, null) : item.MethodInfo.Invoke(this, parameters.ToArray());
        }

        #endregion

        #region Beamer Implements

        public async Task BeamAsync(string methodName)
        {
            await BeamAsync(methodName, null, null);
        }

        public async Task BeamAsync<T1>(string methodName, T1 arg)
        {
            await BeamAsync(methodName,
                new object[] { arg },
                new Type[] { typeof(T1) });
        }

        public async Task BeamAsync<T1, T2>(string methodName, T1 arg1, T2 arg2)
        {
            await BeamAsync(methodName,
                new object[] { arg1, arg2 },
                new Type[] { typeof(T1), typeof(T2) });
        }

        public async Task BeamAsync<T1, T2, T3>(string methodName, T1 arg1, T2 arg2, T3 arg3)
        {
            await BeamAsync(methodName,
                new object[] { arg1, arg2, arg3 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        public async Task BeamAsync<T1, T2, T3, T4>(string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            await BeamAsync(methodName,
                new object[] { arg1, arg2, arg3, arg4 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        }

        public async Task BeamAsync(string methodName, object[] args, Type[] types)
        {
            RPCPayload payload = new RPCPayload
            {
                Parameters = new List<string>(),
                MethodName = methodName
            };

            for (int i = 0; i < args.Length; i++)
            {
                payload.Parameters.Add(Serialize(args[i], types[i]));
            }

            byte[] data = Serialize(payload).GetBytes<UTF8Encoding>();

            await _mServer.SendAllAsync(data).ConfigureAwait(false);
        }

        internal async Task BeamClientAsync(string connectionId, string methodName, object[] args, Type[] types)
        {
            if (!_mServer.HasClient(connectionId))
            {
                return;
            }

            RPCPayload payload = new RPCPayload
            {
                Parameters = new List<string>(),
                MethodName = methodName
            };

            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    payload.Parameters.Add(Serialize(args[i], types[i]));
                }
            }
            await SendPayloadAsync(connectionId, payload).ConfigureAwait(false);
        }

        #endregion
    }
}