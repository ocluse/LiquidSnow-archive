using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Thismaker.Aba.Client.SignalR
{
    /// <summary>
    /// The <see cref="SignalRClientBase{T}"/> is an abstract class with the very basic fundamentals of what
    /// all apps are supposed to contain
    /// </summary>
    public abstract partial class SignalRClientBase<TClient> : ClientBase<TClient> where TClient : SignalRClientBase<TClient>
    {

        #region Properties 
        /// <summary>
        /// The hub endpoint
        /// </summary>
        public string HubEndpoint { get; set; }

        /// <summary>
        /// The connection to the server's hub
        /// </summary>
        protected HubConnection HubConnection { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Fired when the underlying <see cref="HubConnection"/> is reconnecting
        /// </summary>
        public event Action<Exception> HubReconnecting;

        /// <summary>
        /// Fired when the underlying <see cref="HubConnection"/> is reconnected
        /// </summary>
        public event Action<string> HubReconnected;

        /// <summary>
        /// Fired when the underlying <see cref="HubConnection"/> is closed.
        /// Exception will be null in case no error occurred when closing the hub
        /// </summary>
        public event Action<Exception> HubClosed;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes the HttpClient and the HubConnection. Should be called only onces during the lifetime of the application.
        /// This method must be called before accessing any of the Hub and HttpClient methods.
        /// Overriding this method allows you to specify how the HttpClient and HubConnection should be initialized.
        /// </summary>
        public override void MakeApp()
        {
            base.MakeApp();

            HubConnection = new HubConnectionBuilder()
                   .WithAutomaticReconnect()
                   .WithUrl($"{BaseAddress}/{HubEndpoint}/",
                   options =>
                   {
                       options.AccessTokenProvider = HubReadAccessTokenAsync;
                   })
                   .Build();

            HubConnection.Reconnected += OnHubConnectionReconnected;
            HubConnection.Reconnecting += OnHubConnectionReconnecting;
            HubConnection.Closed += OnHubConnectionClosed;

            AutoRenewAccessToken = true;
        }

        #endregion

        #region Private Methods

        private Task OnHubConnectionClosed(Exception arg)
        {
            HubClosed?.Invoke(arg);
            return Task.CompletedTask;
        }

        private Task OnHubConnectionReconnecting(Exception arg)
        {
            HubReconnecting?.Invoke(arg);
            return Task.CompletedTask;
        }

        private Task OnHubConnectionReconnected(string arg)
        {
            HubReconnected?.Invoke(arg);
            return Task.CompletedTask;
        }

        private async Task<string> HubReadAccessTokenAsync()
        {
            if (AccessToken.IsExpired())
            {
                await RenewAccessTokenAsync();
            }

            return AccessToken.Value;
        }

        #endregion

        #region Helpers Implementations
        /// <summary>
        /// Called by the HTTP helpers to deserialize a response. Override to customize how deserialization works
        /// </summary>
        /// <typeparam name="T">The type of required result</typeparam>
        /// <param name="args">The string to deserialize</param>
        protected override T Deserialize<T>(string args)
        {
            return JsonSerializer.Deserialize<T>(args, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// Called by the HTTP helpers, particularly the POST helper to serialize an object to be added to the body of the request
        /// </summary>
        /// <typeparam name="T">The type of object being serialized</typeparam>
        /// <param name="args">The object to serialize</param>
        protected override string Serialize<T>(T args)
        {
            return JsonSerializer.Serialize(args, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        #endregion

        #region Hub

        private List<string> _subscribedHubMethodNames;

        /// <summary>
        /// Subscribes methods marked with the <see cref="SubscribeAttribute"/>. Override 
        /// to customize the behaviour
        /// </summary>
        public virtual void SubscribeHub()
        {
            _subscribedHubMethodNames = new List<string>();

            //get all methods with the subscribe attribute:
            MethodInfo[] methods = GetType()
                .GetMethods();

            foreach (MethodInfo method in methods)
            {
                SubscribeAttribute subAttr = method.GetCustomAttribute<SubscribeAttribute>();
                if (subAttr == null)
                {
                    continue;
                }

                string name = string.IsNullOrEmpty(subAttr.MethodName) ?
                    method.Name : subAttr.MethodName;

                List<Type> types = new List<Type>();
                ParameterInfo[] pInfos = method.GetParameters();

                foreach (ParameterInfo pInfo in pInfos)
                {
                    types.Add(pInfo.ParameterType);
                }

                _ = HubConnection.On(name, types.ToArray(), OnHubCall, method);

                _subscribedHubMethodNames.Add(name);
            }
        }

        private Task OnHubCall(object[] parameters, object state)
        {
            MethodInfo method = (MethodInfo)state;

            _ = method.Invoke(this, parameters);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Unsubscribes methods previously subscribed to through
        /// <see cref="SubscribeHub"/>. Overriding either method changes the behaviour
        /// </summary>
        public virtual void UnsubscribeHub()
        {
            if (_subscribedHubMethodNames == null)
            {
                return;
            }

            foreach (string methodName in _subscribedHubMethodNames)
            {
                HubConnection.Remove(methodName);
            }
        }

        /// <summary>
        /// Attempts to connect to the hub
        /// </summary>
        public async Task ConnectHubAsync()
        {
            await HubConnection.StartAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Attempts to disconnect the hub by stopping it gracefully.
        /// </summary>
        public async Task DisconnectHubAsync()
        {
            await HubConnection.StopAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Send an RPC to the connected hub.
        /// </summary>
        public async Task HubSend(string methodName)
        {
            await HubConnection.SendAsync(methodName).ConfigureAwait(false);
        }

        /// <summary>
        /// Send an RPC to the connected hub.
        /// </summary>
        public async Task HubSend(string methodName, object arg1)
        {
            await HubConnection.SendAsync(methodName, arg1).ConfigureAwait(false);
        }

        /// <summary>
        /// Send an RPC to the connected hub.
        /// </summary>
        public async Task HubSend(string methodName, object arg1, object arg2)
        {
            await HubConnection.SendAsync(methodName, arg1, arg2).ConfigureAwait(false);
        }

        /// <summary>
        /// Send an RPC to the connected hub.
        /// </summary>
        public async Task HubSend(string methodName, object arg1, object arg2, object arg3)
        {
            await HubConnection.SendAsync(methodName, arg1, arg2, arg3).ConfigureAwait(false);
        }

        /// <summary>
        /// Send an RPC to the connected hub. If you wish to send more than four arguments, consider using <see cref="HubConnection"/> directly
        /// </summary>
        public async Task HubSend(string methodName, object arg1, object arg2, object arg3, object arg4)
        {
            await HubConnection.SendAsync(methodName, arg1, arg2, arg3, arg4).ConfigureAwait(false);
        }

        /// <summary>
        /// Unsubscribes a method from the hub
        /// </summary>
        public void UnbindHub(Action action)
        {
            string name = action.Method.Name;
            HubConnection.Remove(name);
        }

        /// <summary>
        /// Unsubscribes a method from the hub
        /// </summary>
        public void UnbindHub<T1>(Action<T1> action)
        {
            string name = action.Method.Name;
            _ = HubConnection.On(name, action);
        }

        /// <summary>
        /// Unsubscribes a method from the hub
        /// </summary>
        public void UnbindHub<T1, T2>(Action<T1, T2> action)
        {
            string name = action.Method.Name;
            _ = HubConnection.On(name, action);
        }

        /// <summary>
        /// Unsubscribes a method from the hub
        /// </summary>
        public void UnbindHub<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            string name = action.Method.Name;
            _ = HubConnection.On(name, action);
        }

        /// <summary>
        /// Unsubscribes a method from the hub
        /// </summary>
        public void UnbindHub<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        {
            string name = action.Method.Name;
            _ = HubConnection.On(name, action);
        }

        /// <summary>
        /// Adds a method to be called when the Hub recieves an RPC of the same name as the method
        /// </summary>
        public void BindHub(Action action)
        {
            string name = action.Method.Name;
            _ = HubConnection.On(name, action);
        }

        /// <summary>
        /// Adds a method to be called when the Hub recieves an RPC of the same name as the method
        /// </summary>
        public void BindHub<T1>(Action<T1> action)
        {
            string name = action.Method.Name;
            _ = HubConnection.On(name, action);
        }

        /// <summary>
        /// Adds a method to be called when the Hub recieves an RPC of the same name as the method
        /// </summary>
        public void BindHub<T1, T2>(Action<T1, T2> action)
        {
            string name = action.Method.Name;
            _ = HubConnection.On(name, action);
        }

        /// <summary>
        /// Adds a method to be called when the Hub recieves an RPC of the same name as the method
        /// </summary>
        public void BindHub<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            string name = action.Method.Name;
            _ = HubConnection.On(name, action);
        }

        /// <summary>
        /// Adds a method to be called when the Hub recieves an RPC of the same name as the method
        /// </summary>
        public void BindHub<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        {
            string name = action.Method.Name;
            _ = HubConnection.On(name, action);
        }


        /// <summary>
        /// Sends a request to the hub and holds the calling thread until a response is received.
        /// To use this method, ensure that the SignalR hub server must sends response after recieving this call through all paths.
        /// </summary>
        /// <typeparam name="TResult">The result expected from the hub. This method is only useful from single results</typeparam>
        /// <param name="requestName">The name of the method to be called on the Server Hub</param>
        /// <param name="responseName">The name of the method expected to be called by the server for response. If null, then the<paramref name="requestName"/> is used instead</param>
        /// <param name="content">The content to send to the method, , if null, no content is sent</param>
        /// <param name="timeOut">If provided, the method throws a <see cref="TaskCanceledException"/> if the time  in miliseconds elapses without receiving a response</param>
        public async Task<TResult> CallHubAsync<TResult>(string requestName, string responseName = null, object content = null, double? timeOut = null)
        {
            if (string.IsNullOrEmpty(responseName))
            {
                responseName = requestName;
            }
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();

            if (timeOut.HasValue)
            {
                System.Timers.Timer timer = new System.Timers.Timer(timeOut.Value)
                {
                    Enabled = true
                };

                void TimerElapsed(object sender, EventArgs e)
                {
                    HubConnection.Remove(responseName);
                    timer.Elapsed -= TimerElapsed;
                    tcs.SetCanceled();
                }
                timer.Elapsed += TimerElapsed;
            }

            void OnHubResponse(TResult result)
            {
                HubConnection.Remove(responseName);
                tcs.SetResult(result);
            }
            _ = HubConnection.On<TResult>(responseName, OnHubResponse);
            if (content == null)
            {
                await HubSend(requestName);
            }
            else
            {
                await HubSend(requestName, content);
            }
            return await tcs.Task;
        }
        #endregion
    }
}