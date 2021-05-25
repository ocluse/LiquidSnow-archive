using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;

namespace Thismaker.Aba.Server.Mercury
{
    public class MercuryUser : IBeamer
    {
        private readonly HashSet<string> _connectionIds;
        private MercuryServer _server;
        public string UserIdentifier { get; private set; }
        public bool Authenticated { get; set; }

        internal MercuryUser(string userId, MercuryServer server)
        {
            _connectionIds = new HashSet<string>();
            _server = server;
            UserIdentifier = userId;
        }

        internal MercuryUser(MercuryUser user)
        {
            _connectionIds = new HashSet<string>(user._connectionIds);
            Authenticated = user.Authenticated;
            _server = user._server;
            UserIdentifier = user.UserIdentifier;
        }

        public void AddConnectionId(string connId)
        {
            if (_connectionIds.Contains(connId)) return;
            _connectionIds.Add(connId);
        }

        public bool HasConnectionId(string connId)
        {
            return _connectionIds.Contains(connId);
        }

        public bool RemoveConnectionId(string connId)
        {
            _connectionIds.Remove(connId);
            return _connectionIds.Count > 0;
        }

        internal void SetServer(MercuryServer server)
        {
            _server = server;
        }

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
            foreach (var connectionId in _connectionIds)
            {
                await _server.BeamClientAsync(connectionId, methodName, args, types);
            }
        }
    }
}
