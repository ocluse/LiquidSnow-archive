using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Thismaker.Aba.Common;

namespace Thismaker.Aba.Client
{
    /// <summary>
    /// The <see cref="ClientBase{T}"/> is an abstract class with the very basic fundamentals of what
    /// all apps are supposed to contain
    /// </summary>
    public abstract class ClientBase<T> where T : ClientBase<T>
    {
        private AccessToken accessToken;

        #region Properties
        public IContext Context { get; set; }

        public static T Instance { get; private set; }

        /// <summary>
        /// The version number of the current <b>local</b> client.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Represents the base url address to the server api endpoint e.g www.liquidsnow.com
        /// </summary>
        public string BaseAddress { get; protected set; }

        /// <summary>
        /// The default address where the Api calls will be made. 
        /// If left null, the endpoint must be included in the requestUri 
        /// when calling <see cref="ApiGetAsync(string)"/> or any of the related methods
        /// </summary>
        public string ApiEndpoint { get; set; }

        public string HubEndpoint { get; set; }

        /// <summary>
        /// The access token that is usually added as an authorization header
        /// to the app's <see cref="HubConnection"/> and <see cref="HttpClient"/>.
        /// </summary>
        public AccessToken AccessToken 
        {
            get
            {
                return accessToken;
            } 
            protected set
            {
                if (accessToken != null && accessToken.Kind==AccessTokenKind.Custom)
                {
                    HttpClient.DefaultRequestHeaders.Remove(accessToken.Key);
                }
                accessToken = value;

                if (accessToken.Kind == AccessTokenKind.Custom)
                {
                    HttpClient.DefaultRequestHeaders.Add(accessToken.Key, accessToken.Value);
                }
                else
                {
                    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(accessToken.Key, accessToken.Value);
                }
            } 
        }

        /// <summary>
        /// The client used to access the server api
        /// </summary>
        protected HttpClient HttpClient { get; set; }

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

        #region Abstract Methods
        /// <summary>
        /// When overriden in a derived class, starts the ClientApp, 
        /// allowing for performing basic tasks.
        /// Should be called once <see cref="MakeApp"/> has been called.
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public abstract Task Start(IProgress<string> progress);

        /// <summary>
        /// When overidden in a derived class, returns the version of the cloud client.
        /// </summary>
        /// <returns></returns>
        public abstract Task<string> GetCloudVersion();
        #endregion

        #region Initialization
        /// <summary>
        /// Should be called before accessing the Client-App's methods. Initializes the basics.
        /// Where a costom <see cref="AccessToken"/> is used, this should be called again to refresh the AccessToken.
        /// Note that this destroys previous bindings to the Hub and HttpClient, and therefore those must be refreshed.
        /// </summary>
        public virtual void MakeApp()
        {
            HttpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
            HubConnection = new HubConnectionBuilder()
                   .WithAutomaticReconnect()
                   .WithUrl($"{BaseAddress}/{HubEndpoint}/",
                   options =>
                   {
                       options.AccessTokenProvider = GetAccessTokenAsync;
                   })
                   .Build();

            HubConnection.Reconnected += OnHubConnectionReconnected;
            HubConnection.Reconnecting += OnHubConnectionReconnecting;
            HubConnection.Closed += OnHubConnectionClosed;
        }

        /// <summary>
        /// Makes the app the singleton that can be easily accessed by <see cref="ClientBase{T}.Instance"/>
        /// Can be used for persistance accross the environment's lifetime.
        /// </summary>
        public void MakeSingleton()
        {
            Instance = (T)this;
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

        private Task<string> GetAccessTokenAsync()
        {
            return Task.FromResult(AccessToken?.Value);
        }
        #endregion

        #region Api Methods
        /// <summary>
        /// Calls the HTTP GET on the server's Api
        /// </summary>
        /// <param name="requestUri">The specific request uri, will be combined with the path info</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ApiGetAsync(string requestUri)
        {
            try
            {
                return await HttpClient.GetAsync(ApiEndpoint==null?$"/{requestUri}"
                    :$"/{ApiEndpoint}/{requestUri}");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Sends a HTTP GET request to the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint that will be attached to the base address and called.</param>
        /// <param name="requestUri">The request Url</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> EndpointGetAsync(string endpoint, string requestUri)
        {
            try
            {
                return await HttpClient.GetAsync($"{endpoint}/{requestUri}");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Sends a HTTP GET request to the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint that will be attached to the base address and called.</param>
        /// <param name="requestUri">The request Url</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> EndpointPostAsync(string endpoint, string requestUri, HttpContent content)
        {
            try
            {
                return await HttpClient.PostAsync($"{endpoint}/{requestUri}", content);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Calls a HTTP POST on the server's Api
        /// </summary>
        /// <param name="requestUri">The specific uri to the post resource</param>
        /// <param name="content">The content to send to the server</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ApiPostAsync(string requestUri, HttpContent content)
        {
            try
            {
                return await HttpClient.PostAsync(ApiEndpoint == null ? $"/{requestUri}"
                    : $"/{ApiEndpoint}/{requestUri}", content);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Hub

        public abstract void SubscribeHub();

        public abstract void UnsubscribeHub();

        public async Task ConnectHub()
        {
            try
            {
                await HubConnection.StopAsync();
            }
            catch
            {
                //do nothing
            }

            try
            {
                await HubConnection.StartAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task HubSend(string methodName)
        {
            await HubConnection.SendAsync(methodName);
        }

        public async Task HubSend<T1>(string methodName, T1 arg1)
        {
            await HubConnection.SendAsync(methodName, arg1);
        }

        public async Task HubSend<T1, T2>(string methodName, T1 arg1, T2 arg2)
        {
            await HubConnection.SendAsync(methodName, arg1, arg2);
        }

        public async Task HubSend<T1,T2,T3>(string methodName, T1 arg1, T2 arg2, T3 arg3)
        {
            await HubConnection.SendAsync(methodName, arg1, arg2, arg3);
        }

        public void UnbindHub(Action action)
        {
            var name = action.Method.Name;
            HubConnection.Remove(name);
        }

        public void UnbindHub<T1>(Action<T1> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }
        
        public void UnbindHub<T1,T2>(Action<T1,T2> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }
        
        public void UnbindHub<T1,T2,T3>(Action<T1,T2,T3> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        public void BindHub(Action action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        public void BindHub<T1>(Action<T1> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        public void BindHub<T1,T2>(Action<T1,T2> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        public void BindHub<T1,T2,T3>(Action<T1,T2,T3> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        public async Task<Result> CallHubAsync<Result>(string methodName)
        {
            var tcs = new TaskCompletionSource<Result>();
            HubConnection.On<Result>(methodName, result =>
            {
                HubConnection.Remove(methodName);
                tcs.SetResult(result);
            });
            await HubConnection.SendAsync(methodName);

            return await tcs.Task;
        }

        public async Task<Result> CallHubAsync<Result>(IPackage package, [CallerMemberName] string methodName = null)
        {
            var tcs = new TaskCompletionSource<Result>();
            HubConnection.On<Result>(methodName, result =>
            {
                HubConnection.Remove(methodName);
                tcs.SetResult(result);
            });
            await HubConnection.SendAsync(methodName, package.ToJson());

            return await tcs.Task;
        }

        #endregion
    }
}