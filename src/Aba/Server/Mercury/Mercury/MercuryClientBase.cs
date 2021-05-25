using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;

namespace Thismaker.Aba.Server.Mercury
{
    public class MercuryClientBase : IBeamer
    {
        protected readonly string _connectionId;
        private readonly MercuryServer _server;

        internal MercuryClientBase(string connectionId, MercuryServer server)
        {
            _connectionId = connectionId; 
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
            await _server.BeamClientAsync(_connectionId, methodName, args, types);
        }
    }
}
