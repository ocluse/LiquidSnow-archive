﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Aba.Common.Mercury;

namespace Thismaker.Aba.Client.Mercury
{
    public abstract partial class MercuryClientBase<TClient> : IBeamer
    {
        private readonly Dictionary<string, InvocationHandler> _subs
            = new Dictionary<string, InvocationHandler>();

        #region Receiving

        private async void PayloadReceived(RPCPayload payload)
        {
            //See if the payload wants us to close the connection
            if (payload.MethodName == Globals.CloseConnection)
            {
                Exception ex = null;
                if (payload.Parameters.Count > 0)
                {
                    ex = new OperationCanceledException(payload.Parameters[0]);
                }
                await _mClient.DisconnectAsync(ex).ConfigureAwait(false);
                return;
            }

            if (!_subs.ContainsKey(payload.MethodName)) return;

            InvocationHandler handler = _subs[payload.MethodName];

            if (handler.Types == null || handler.Types.Count == 0)
            {
                handler.Invoke(null);
            }
            else
            {
                List<object> parameters = new List<object>();
                for (int i = 0; i < payload.Parameters.Count; i++)
                {
                    parameters.Add(Deserialize(payload.Parameters[i], handler.Types[i]));
                }
                handler.Invoke(parameters.ToArray());
            }
        }

        public void Unsubscribe(string methodName)
        {
            _subs.Remove(methodName);
        }

        /// <summary>
        /// Subscribe a method to the RPC Beam, invoking the method when a payload of the same name is received.
        /// </summary>
        public void Subscribe(string methodName, object target, MethodInfo method, Type[] types)
        {
            InvocationHandler handler;

            if (_subs.ContainsKey(methodName))
            {
                handler = _subs[methodName];
            }
            else
            {
                handler = new InvocationHandler()
                {
                    Binds = new List<InvocationBind>(),
                    Types = types == null ? null : new List<Type>(types)
                };

                _subs.Add(methodName, handler);
            }

            InvocationBind bind = new InvocationBind
            {
                Info = method,
                Target = target
            };

            handler.Binds.Add(bind);
        }

        /// <summary>
        /// Subscribe a method to the RPC Beam, invoking the method when a payload of the same name is received.
        /// </summary>
        public void Subscribe(Action action, string methodName = null)
        {
            if (methodName == null)
            {
                methodName = action.Method.Name;
            }

            Subscribe(methodName, action.Target, action.Method, null);
        }

        /// <summary>
        /// Subscribe a method to the RPC Beam, invoking the method when a payload of the same name is received.
        /// </summary>
        public void Subscribe<T1>(Action<T1> action, string methodName = null)
        {
            if (methodName == null)
            {
                methodName = action.Method.Name;
            }

            Subscribe(methodName, action.Target, action.Method, new Type[] { typeof(T1) });
        }

        /// <summary>
        /// Subscribe a method to the RPC Beam, invoking the method when a payload of the same name is received.
        /// </summary>
        public void Subscribe<T1, T2>(Action<T1, T2> action, string methodName = null)
        {
            if (methodName == null)
            {
                methodName = action.Method.Name;
            }

            Subscribe(methodName, action.Target, action.Method,
                new Type[] { typeof(T1), typeof(T2) });
        }

        /// <summary>
        /// Subscribe a method to the RPC Beam, invoking the method when a payload of the same name is received.
        /// </summary>
        public void Subscribe<T1, T2, T3>(Action<T1, T2, T3> action, string methodName = null)
        {
            if (methodName == null)
            {
                methodName = action.Method.Name;
            }

            Subscribe(methodName, action.Target, action.Method,
                new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        /// <summary>
        /// Subscribe a method to the RPC Beam, invoking the method when a payload of the same name is received.
        /// </summary>
        public void Subscribe<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, string methodName = null)
        {
            if (methodName == null)
            {
                methodName = action.Method.Name;
            }

            Subscribe(methodName, action.Target, action.Method,
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        }

        class InvocationBind
        {
            public MethodInfo Info { get; set; }
            public object Target { get; set; }
        }

        class InvocationHandler
        {
            public List<InvocationBind> Binds { get; set; }

            public List<Type> Types { get; set; }

            public void Invoke(object[] parameters)
            {

                foreach (InvocationBind bind in Binds)
                {
                    bind.Info.Invoke(bind.Target, parameters);
                }
            }
        }

        #endregion

        #region Sending

        private async Task SendPayloadAsync(RPCPayload payload)
        {
            byte[] bytes = Serialize(payload).GetBytes<UTF8Encoding>();
            await _mClient.SendAsync(bytes).ConfigureAwait(false);
        }

        public async Task BeamAsync(string methodName, object[] args, Type[] types)
        {
            RPCPayload payload = new RPCPayload
            {
                MethodName = methodName,
                Parameters = new List<string>()
            };

            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    payload.Parameters.Add(Serialize(args[i], types[i]));
                }
            }
            await SendPayloadAsync(payload);
        }

        public async Task BeamAsync(string methodName)
        {
            await BeamAsync(methodName, null, null);
        }

        public async Task BeamAsync<T1>(string methodName, T1 arg)
        {
            await BeamAsync(methodName, new object[] { arg },
                new Type[] { typeof(T1) });
        }

        public async Task BeamAsync<T1, T2>(string methodName, T1 arg1, T2 arg2)
        {
            await BeamAsync(methodName, new object[] { arg1, arg2 },
                new Type[] { typeof(T1), typeof(T2) });
        }

        public async Task BeamAsync<T1, T2, T3>(string methodName, T1 arg1, T2 arg2, T3 arg3)
        {
            await BeamAsync(methodName, new object[] { arg1, arg2, arg3 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        public async Task BeamAsync<T1, T2, T3, T4>(string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            await BeamAsync(methodName, new object[] { arg1, arg2, arg3, arg4 },
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        }

        #endregion
    }
}