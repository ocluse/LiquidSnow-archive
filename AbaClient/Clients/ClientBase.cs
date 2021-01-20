
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Aba.Common;

namespace Thismaker.Aba.Client.Clients
{
    /// <summary>
    /// The <see cref="ClientBase{T}"/> is an abstract class with the very basic fundamentals of what
    /// all apps are supposed to contain
    /// </summary>
    public abstract class ClientBase<T> where T : ClientBase<T>
    {
        private AccessToken accessToken;

        public IContext Context { get; set; }

        public static T Instance { get; private set; }

        /// <summary>
        /// The version number of the current <b>local</b> client.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Represents the base url address to the server api endpoint e.g www.liquidsnow/api/main
        /// </summary>
        public string BaseAddress { get; protected set; }

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

        #region Events
        public event Action<Exception> HubReconnecting;
        public event Action<string> HubReconnected;
        public event Action<Exception> HubClosed;
        #endregion

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

        public virtual void Configure(IConfiguration config)
        {
            var abaSection = config.GetSection("AbaClient");
            BaseAddress = abaSection.GetSection("BaseAddress").Value;
            ApiEndpoint = abaSection.GetSection("ApiEndpoint").Value;
            HubEndpoint = abaSection.GetSection("HubEndpoint").Value;

            HttpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
            HubConnection = new HubConnectionBuilder()
                   .WithAutomaticReconnect()
                   .WithUrl($"{BaseAddress}/{HubEndpoint}/",
                   options =>
                   {
                       options.AccessTokenProvider = GetHubToken;
                   })
                   .Build();

            HubConnection.Reconnected += HubConnection_Reconnected;
            HubConnection.Reconnecting += HubConnection_Reconnecting;
            HubConnection.Closed += HubConnection_Closed;
        }

        private Task HubConnection_Closed(Exception arg)
        {
            HubClosed?.Invoke(arg);
            return Task.CompletedTask;
        }

        private Task HubConnection_Reconnecting(Exception arg)
        {
            HubReconnecting?.Invoke(arg);
            return Task.CompletedTask;
        }

        private Task HubConnection_Reconnected(string arg)
        {
            HubReconnected?.Invoke(arg);
            return Task.CompletedTask;
        }

        private Task<string> GetHubToken()
        {
            return Task.FromResult(AccessToken?.Value);

            //if (AccessToken != null)
            //{
            //    if (AccessToken.Kind == AccessTokenKind.Custom)
            //    {
            //        options.Headers.Add(accessToken.Key, accessToken.Value);
            //    }
            //    else
            //    {
            //        options.AccessTokenProvider = () => Task.FromResult(AccessToken.Value);
            //    }
            //}
        }

        public void MakeSingleton()
        {
            Instance = (T)this;
        }

        public abstract Task Start(IProgress<string> progress);

        public abstract Task<string> GetCloudVersion();

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


