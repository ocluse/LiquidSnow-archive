using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;

namespace Thismaker.Aba.Server.Mercury
{
    public class Group : IBeamer
    {
        #region Private Methods
        private readonly string _groupName;
        private readonly MercuryServer _server;
        private readonly HashSet<string> _clients;
        #endregion

        #region Properties
        public string GroupName
        {
            get => _groupName;
        }
        #endregion

        public Group(MercuryServer server, string groupName)
        {
            _clients = new HashSet<string>();
            _groupName = groupName;
            _server = server;
        }

        #region Groups

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
            foreach (string client in _clients)
            {
                await _server.BeamClientAsync(client, methodName, args, types);
            }
        }

        internal void RemoveFromGroup(string connectionId)
        {
            _ = _clients.Remove(connectionId);
        }

        internal void AddConnectionId(string connectionId)
        {
            _ = _clients.Add(connectionId);
        }

        #endregion

        #region GroupExcept

        public async Task BeamExceptAsync(string excludeId, string methodName)
        {
            await BeamExceptAsync(excludeId, methodName, null, null);
        }

        public async Task BeamExceptAsync<T1>(string excludeId, string methodName, T1 arg)
        {
            await BeamExceptAsync(excludeId, methodName,
                new object[] { arg },
                new Type[] { typeof(T1) });
        }

        public async Task BeamExceptAsync<T1, T2>(string excludeId, string methodName, T1 arg1, T2 arg2)
        {
            await BeamExceptAsync(excludeId, methodName,
                new object[] { arg1, arg2 },
                new Type[] { typeof(T1), typeof(T2) });
        }

        public async Task BeamExceptAsync<T1, T2, T3>(string excludeId, string methodName, T1 arg1, T2 arg2, T3 arg3)
        {
            await BeamExceptAsync(excludeId, methodName,
                new object[] { arg1, arg2, arg3 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        public async Task BeamExceptAsync<T1, T2, T3, T4>(string excludeId, string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            await BeamExceptAsync(excludeId, methodName,
                new object[] { arg1, arg2, arg3, arg4 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        }

        public async Task BeamExceptAsync(string excludeId, string methodName, object[] args, Type[] types)
        {
            foreach (string client in _clients)
            {
                if (excludeId == client)
                {
                    continue;
                }

                await _server.BeamClientAsync(client, methodName, args, types);
            }
        }

        #endregion

    }
}
