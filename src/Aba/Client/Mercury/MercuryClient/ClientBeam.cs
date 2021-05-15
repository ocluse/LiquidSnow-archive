using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Thismaker.Aba.Common.Mercury;

namespace Thismaker.Aba.Client.Mercury
{
    public abstract partial class MercuryClient<TClient>
    {
        private readonly Dictionary<string, InvocationHandler> _subs
            = new Dictionary<string, InvocationHandler>();

        #region Receiving

        private void PayloadReceived(RPCPayload payload)
        {
            if (!_subs.ContainsKey(payload.MethodName)) return;

            var handler = _subs[payload.MethodName];

            var parameters = new List<object>();

            for (int i = 0; i < payload.Parameters.Count; i++)
            {
                parameters.Add(Deserialize(payload.Parameters[i], handler.Types[i]));
            }

            handler.Invoke(parameters.ToArray());
        }

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
                    Types = new List<Type>(types)
                };

                _subs.Add(methodName, handler);
            }

            var bind = new InvocationBind
            {
                Info = method,
                Target = target
            };

            handler.Binds.Add(bind);
        }

        public void Subscribe<T1>(Action<T1> action, string methodName=null)
        {
            if (methodName == null)
            {
                methodName = action.Method.Name;
            }

            Subscribe(methodName, action.Target, action.Method, new Type[] { typeof(T1)});
        }

        public void Subscribe<T1, T2>(Action<T1, T2> action, string methodName = null)
        {
            if (methodName == null)
            {
                methodName = action.Method.Name;
            }

            Subscribe(methodName, action.Target, action.Method,
                new Type[] { typeof(T1), typeof(T2)});
        }

        public void Subscribe<T1, T2, T3>(Action<T1, T2, T3> action, string methodName = null)
        {
            if (methodName == null)
            {
                methodName = action.Method.Name;
            }

            Subscribe(methodName, action.Target, action.Method,
                new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

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

                foreach(var bind in Binds)
                {
                    bind.Info.Invoke(bind.Target, parameters);
                }
            }
        }

        #endregion

        #region Sending

        private void SendPayload(RPCPayload payload)
        {
            var bytes = Serialize(payload).GetBytes<UTF8Encoding>();
            _mClient.SendAsync(bytes);
        }

        public void Beam(string methodName, object[] args, Type[] types)
        {
            var payload = new RPCPayload
            {
                MethodName = methodName,
                Parameters = new List<string>()
            };

            for (int i = 0; i < args.Length; i++)
            {
                payload.Parameters.Add(Serialize(args[i], types[i]));
            }

            SendPayload(payload);
        }

        public void Beam<T1>(string methodName, T1 arg)
        {
            Beam(methodName, new object[] { arg }, 
                new Type[] { typeof(T1) });
        }

        public void Beam<T1, T2>(string methodName, T1 arg1, T2 arg2)
        {
            Beam(methodName, new object[] { arg1, arg2 }, 
                new Type[] { typeof(T1), typeof(T2) });
        }

        public void Beam<T1, T2, T3>(string methodName, T1 arg1, T2 arg2, T3 arg3)
        {
            Beam(methodName, new object[] { arg1, arg2, arg3 }, 
                new Type[] { typeof(T1), typeof(T2), typeof(T3) });
        }

        public void Beam<T1, T2, T3, T4>(string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Beam(methodName, new object[] { arg1, arg2, arg3, arg4 }, 
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4)});
        }

        #endregion
    }
}