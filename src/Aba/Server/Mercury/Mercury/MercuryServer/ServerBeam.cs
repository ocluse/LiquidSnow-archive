using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;
using Thismaker.Mercury;

namespace Thismaker.Aba.Server.Mercury
{
    public partial class MercuryServer
    {
        class CacheItem
        {
            public bool Authorize { get; set; }
            public bool InjectPrincipal { get; set; }
            public MethodInfo MethodInfo { get; set; }
            public List<Type> Types { get; set; }
            public List<string> Scopes { get; set; }
        }

        private Dictionary<string, CacheItem> _methodCache;

        private Dictionary<string, Group> _groups;

        private async void PayloadReceived(RPCPayload payload)
        {
            if (_methodCache == null)
            {
                _methodCache = new Dictionary<string, CacheItem>();

                var methods=GetType().GetMethods();

                foreach(var method in methods)
                {
                    var beamable= method.GetCustomAttribute<BeamableAttribute>();
                    if (beamable == null) continue;

                    string name = string.IsNullOrEmpty(beamable.MethodName) ?
                        method.Name : beamable.MethodName;

                    var types = new List<Type>();
                    var pInfos = method.GetParameters();
                    
                    foreach(var pInfo in pInfos)
                    {
                        types.Add(pInfo.ParameterType);
                    }

                    var cache = new CacheItem 
                    { 
                        MethodInfo = method, 
                        Types = types , 
                        Authorize=beamable.Authorized
                    };

                    if (cache.Types[0] == typeof(IPrincipal))
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

            if (!_methodCache.ContainsKey(payload.MethodName)) return;

            var item = _methodCache[payload.MethodName];
            
            var parameters = new List<object>();

            //first, validate the access token:
            if (item.Authorize)
            {
                try
                {
                    IPrincipal principal = await ValidateAccessToken(payload.AccessToken, item.Scopes);

                    if (principal == null) return;

                    if (item.InjectPrincipal)
                    {
                        parameters.Add(principal);
                    }
                }
                catch
                {
                    return;
                }
            }

            for(int i=0; i<payload.Parameters.Count; i++)
            {
                parameters.Add(Deserialize(payload.Parameters[i], item.Types[i]));
            }
            item.MethodInfo.Invoke(this, parameters.ToArray());
        }

        #region All Clients

        public async Task BeamClientsAsync<T1>(string methodName, T1 arg)
        {
            await BeamClientsAsync(methodName, 
                new object[] { arg }, 
                new Type[] { typeof(T1) });
        }

        public async Task BeamClientsAsync<T1, T2>(string methodName, T1 arg1, T2 arg2)
        {
            await BeamClientsAsync(methodName,
                new object[] { arg1, arg2 },
                new Type[] { typeof(T1), typeof(T2)});
        }

        public async Task BeamClientsAsync<T1, T2, T3>(string methodName, T1 arg1, T2 arg2, T3 arg3)
        {
            await BeamClientsAsync(methodName,
                new object[] { arg1, arg2, arg3 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        public async Task BeamClientsAsync<T1, T2, T3, T4>(string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            await BeamClientsAsync(methodName,
                new object[] { arg1, arg2, arg3, arg4 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        }

        public async Task BeamClientsAsync(string methodName, object[] args, Type[] types)
        {
            var payload = new RPCPayload
            {
                Parameters = new List<string>(),
                MethodName = methodName
            };

            for (int i = 0; i < args.Length; i++)
            {
                payload.Parameters.Add(Serialize(args[i], types[i]));
            }

            var data = Serialize(payload).GetBytes<UTF8Encoding>();

            await _mServer.SendAllAsync(data).ConfigureAwait(false);
        }

        #endregion

        #region Specific Client

        public async Task BeamClientAsync<T1>(string connectionId, string methodName, T1 arg)
        {
            await BeamClientAsync(connectionId, methodName,
                new object[] { arg },
                new Type[] { typeof(T1) });
        }

        public async Task BeamClientAsync<T1, T2>(string connectionId, string methodName, T1 arg1, T2 arg2)
        {
            await BeamClientAsync(connectionId, methodName,
                new object[] { arg1, arg2 },
                new Type[] { typeof(T1), typeof(T2) });
        }

        public async Task BeamClientAsync<T1, T2, T3>(string connectionId, string methodName, T1 arg1, T2 arg2, T3 arg3)
        {
            await BeamClientAsync(connectionId, methodName,
                new object[] { arg1, arg2, arg3 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        public async Task BeamClientAsync<T1, T2, T3, T4>(string connectionId, string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            await BeamClientAsync(connectionId, methodName,
                new object[] { arg1, arg2, arg3, arg4 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        }

        public async Task BeamClientAsync(string connectionId, string methodName, object[] args, Type[] types)
        {
            if (!_mServer.HasClient(connectionId)) return;

            var payload = new RPCPayload
            {
                Parameters = new List<string>(),
                MethodName = methodName
            };

            for (int i = 0; i < args.Length; i++)
            {
                payload.Parameters.Add(Serialize(args[i], types[i]));
            }

            await _mServer.SendAsync(connectionId, Serialize(payload).GetBytes<UTF8Encoding>());

        }

        #endregion

        #region Groups

        public async Task BeamGroupAsync<T1>(string groupName, string methodName, T1 arg)
        {
            await BeamGroupAsync(groupName, methodName,
                new object[] { arg },
                new Type[] { typeof(T1) });
        }

        public async Task BeamGroupAsync<T1, T2>(string groupName, string methodName, T1 arg1, T2 arg2)
        {
            await BeamGroupAsync(groupName, methodName,
                new object[] { arg1, arg2 },
                new Type[] { typeof(T1), typeof(T2) });
        }

        public async Task BeamGroupAsync<T1, T2, T3>(string groupName, string methodName, T1 arg1, T2 arg2, T3 arg3)
        {
            await BeamGroupAsync(groupName, methodName,
                new object[] { arg1, arg2, arg3 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        public async Task BeamGroupAsync<T1, T2, T3, T4>(string groupName, string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            await BeamGroupAsync(groupName, methodName,
                new object[] { arg1, arg2, arg3, arg4 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        }

        public async Task BeamGroupAsync(string groupName, string methodName, object[] args, Type[] types)
        {
            if (_groups == null || !_groups.ContainsKey(groupName))
            {
                return;
            }
            var payload = new RPCPayload
            {
                Parameters = new List<string>(),
                MethodName = methodName
            };

            for (int i = 0; i < args.Length; i++)
            {
                payload.Parameters.Add(Serialize(args[i], types[i]));
            }

            var group = _groups[groupName];

            foreach (var client in group.Clients)
            {
                await BeamClientAsync(client, methodName, args, types);
            }
        }

        public void RemoveFromGroup(string connectionId, string groupName)
        {
            if (_groups == null || !_groups.ContainsKey(groupName)) return;

            var group = _groups[connectionId];
            group.Clients.Remove(connectionId);

            if (group.Clients.Count == 0)
            {
                _groups.Remove(groupName);
            }
        }

        public void AddToGroup(string connectionId, string groupName)
        {
            if (!_mServer.HasClient(connectionId)) return;

            if (_groups == null) _groups = new Dictionary<string, Group>();

            Group group;

            if (_groups.ContainsKey(groupName))
            {
                group = _groups[groupName];
            }
            else
            {
                group = new Group(groupName);
            }

            if (group.Clients.Contains(connectionId)) return;

            group.Clients.Add(connectionId);
        }

        #endregion
    }

    class Group
    {
        private readonly string _groupName;

        public Group(string groupName)
        {
            Clients = new List<string>();
            _groupName = groupName;
        }

        public string GroupName
        {
            get => _groupName;
        }
        public List<string> Clients { get; set; }
    }
}