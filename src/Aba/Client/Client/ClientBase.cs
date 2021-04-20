using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Thismaker.Aba.Common;

namespace Thismaker.Aba.Client
{
    /// <summary>
    /// The <see cref="ClientBase{T}"/> is an abstract class with the very basic fundamentals of what
    /// all apps are supposed to contain
    /// </summary>
    public abstract partial class ClientBase<TClient> where TClient : ClientBase<TClient>
    {

        #region Properties
        public virtual IContext Context { get; set; }

        public static TClient Instance { get; private set; }

        /// <summary>
        /// The version number of the current <b>local</b> client.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Represents the base url address to the server api endpoint e.g www.liquidsnow.com
        /// </summary>
        public string BaseAddress { get; set; }

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
        public AccessToken AccessToken { get; set; }

        /// <summary>
        /// The client used to access the server api
        /// </summary>
        protected HttpClient HttpClient { get; set; }

        /// <summary>
        /// The connection to the server's hub
        /// </summary>
        protected HubConnection HubConnection { get; set; }

        public virtual Func<Task<string>> ReadAccessToken { get; set; }

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
                       options.AccessTokenProvider = ReadHubAccessToken;
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
            Instance = (TClient)this;
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

        protected virtual async Task<string> ReadHubAccessToken()
        {
            if (ReadAccessToken == null)
            {
                return AccessToken?.Value;
            }
            else
            {
                return await ReadAccessToken.Invoke();
            }
        }

        #endregion

        #region Api Methods
        /// <summary>
        /// Prepares the HttpClient by setting the appropriate request header, such as Bearer token, before calling the ServerMethods.
        /// This method is always run before all API Methods such as <see cref="ApiGetAsync(string, bool)"/>
        /// </summary>
        /// <param name="secured">If true, the http client sends along an Authorization or other authentication headers, otherwise, it removes them</param>
        /// <returns></returns>
        protected virtual async Task ReadyHttpClientForRequest(bool secured)
        {
            

            if (secured)
            {
                var accessToken = ReadAccessToken != null ? await ReadAccessToken.Invoke() : AccessToken.Value;
                if (AccessToken.Kind == AccessTokenKind.Custom)
                {
                    HttpClient.DefaultRequestHeaders.Remove(AccessToken.Key);
                    HttpClient.DefaultRequestHeaders.Add(AccessToken.Key, accessToken);
                }
                else
                {
                    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AccessToken.Key, accessToken);
                }
            }
            else
            {
                if (AccessToken?.Kind == AccessTokenKind.Custom)
                {
                    HttpClient.DefaultRequestHeaders.Remove(AccessToken.Key);
                }
                else
                {
                    HttpClient.DefaultRequestHeaders.Remove("Authorization");
                }
            }
        }

        /// <summary>
        /// Calls the HTTP GET on the server's Api
        /// </summary>
        /// <param name="requestUri">The specific request uri, will be combined with the path info</param>
        /// <param name="secured">If true, ensures that the method adds the <see cref="AccessToken"/> to the request for a secured resource</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ApiGetAsync(string requestUri, bool secured=true)
        {
            //Prep the HttpClient
            await ReadyHttpClientForRequest(secured);

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
        /// A simpler way to call a HTTP GET on the APIs endpoint. Deserializes, by JSON the content of the response
        /// </summary>
        /// <typeparam name="TResult">The type to deserialize the content to</typeparam>
        /// <param name="requestUri">The specific resource Uri on the endpoint</param>
        /// <param name="secured">If true, ensures that the method adds the <see cref="AccessToken"/> to the request for a secured resource</param>
        /// <returns>The JSON deserialized content/body of the response from the API</returns>
        public async Task<TResult>  ApiGetSimpleAsync<TResult>(string requestUri, bool secured = true)
        {
            try
            {
                var response = await ApiGetAsync(requestUri, secured);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return FromJson<TResult>(json);
                }
                else
                {
                    throw new ClientException(response.StatusCode.ToString(), ExceptionKind.RegisterFailure);
                }
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
        /// <param name="secured">If true, ensures that the method adds the <see cref="AccessToken"/> to the request for a secured resource</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ApiPostAsync(string requestUri, HttpContent content, bool secured = true)
        {
            //Prep the HttpClient
            await ReadyHttpClientForRequest(secured);

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

        /// <summary>
        /// A simpler way to make a HTTP POST on the Api endpoint that makes it easier to work with.
        /// </summary>
        /// <typeparam name="TContent">The type of the provided <paramref name="content"/></typeparam>
        /// <typeparam name="TResult">The type of the expected result from the method</typeparam>
        /// <param name="requestUri">The specific uri of the post resource</param>
        /// <param name="content">The content to be included in the body of the request</param>
        /// <param name="secured">If true, ensures that an Authorization header, 
        /// or custom auth header is added depending on the <see cref="AccessToken"/></param>
        /// <param name="serializeContent">If true, the content is serialized first by use of the 
        /// <see cref="GetStringContent{T}(T)"/> method. Otherwise, the content <b>must</b> be of type <see cref="HttpContent"/></param>
        /// <param name="deserializeResult">If true, the method first deserializes the Content
        /// (assumes it to be JSON unless <see cref="FromJson{T}(string)"/> has been overriden)  of the <b>response</b> message</param>
        /// <returns>A <see cref="HttpResponseMessage"/> or generic type depending on the params</returns>
        /// <exception cref="ClientException">When <paramref name="deserializeResult"/> is true and the request failed</exception>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<TResult> ApiPostSimpleAsync<TContent, TResult>(string requestUri, TContent content, bool secured = true, bool serializeContent=true, bool deserializeResult=true)
        {
            //Request validation:
            if(!serializeContent)
            {
                var contentType = typeof(TContent);
                if (!(contentType.IsSubclassOf(typeof(HttpContent)) || contentType == typeof(HttpContent)))
                {
                    throw new InvalidOperationException("Content provided must be of type/derive from HttpContent if serializeContent is set to false");
                }
            }

            if (!deserializeResult)
            {
                var resultType = typeof(TResult);
                if (!(resultType.IsSubclassOf(typeof(HttpResponseMessage)) || resultType == typeof(HttpResponseMessage)))
                {
                    throw new InvalidOperationException("Generic TResult provided must be of type/derive from HttpResponseMessage if serializeResult is set to false");
                }
            }

            try
            {
                var response = serializeContent ?
                    await ApiPostAsync(requestUri, GetStringContent(content), secured) :
                    await ApiPostAsync(requestUri, (HttpContent)(object)content, secured);

                if (!deserializeResult)
                {
                    return (TResult)(object)response;
                }

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return FromJson<TResult>(json);
                }
                else
                {
                    throw new ClientException(response.StatusCode.ToString(), ExceptionKind.RequestFailed);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<HttpResponseMessage> EndpointGetAsync(string endpoint, string requestUri, bool secured=true)
        {
            //Prep the HttpClient
            await ReadyHttpClientForRequest(secured);

            try
            {
                return await HttpClient.GetAsync($"{endpoint}/{requestUri}");
            }
            catch
            {
                throw;
            }
        }

        public async Task<TResult> EndpointGetSimpleAsync<TResult>(string endpoint, string requestUri, bool secured = true)
        {
            try
            {
                var response = await EndpointGetAsync(endpoint, requestUri, secured);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return FromJson<TResult>(json);
                }
                else
                {
                    throw new ClientException(response.StatusCode.ToString(), ExceptionKind.RegisterFailure);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<HttpResponseMessage> EndpointPostAsync(string endpoint, string requestUri, HttpContent content, bool secured=true)
        {
            //Prep the HttpClient
            await ReadyHttpClientForRequest(secured);

            try
            {
                return await HttpClient.PostAsync($"{endpoint}/{requestUri}", content);
            }
            catch
            {
                throw;
            }
        }

        public async Task<TResult> EndpointPostSimpleAsync<TContent, TResult>(string endpoint, string requestUri, TContent content, bool secured = true, bool serializeContent = true, bool deserializeResult = true)
        {
            //Request validation:
            if (!serializeContent)
            {
                var contentType = typeof(TContent);
                if (!(contentType.IsSubclassOf(typeof(HttpContent)) || contentType == typeof(HttpContent)))
                {
                    throw new InvalidOperationException("Content provided must be of type/derive from HttpContent if serializeContent is set to false");
                }
            }

            if (!deserializeResult)
            {
                var resultType = typeof(TResult);
                if (!(resultType.IsSubclassOf(typeof(HttpResponseMessage)) || resultType == typeof(HttpResponseMessage)))
                {
                    throw new InvalidOperationException("Generic TResult provided must be of type/derive from HttpResponseMessage if serializeResult is set to false");
                }
            }

            try
            {
                var response = serializeContent ?
                    await EndpointPostAsync(endpoint, requestUri, GetStringContent(content), secured) :
                    await EndpointPostAsync(endpoint, requestUri, (HttpContent)(object)content, secured);

                if (!deserializeResult)
                {
                    return (TResult)(object)response;
                }

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return FromJson<TResult>(json);
                }
                else
                {
                    throw new ClientException(response.StatusCode.ToString(), ExceptionKind.RequestFailed);
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region Helpers
        protected virtual T FromJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        protected virtual string ToJson<T>(T json)
        {
            return JsonSerializer.Serialize(json);
        }

        protected virtual StringContent GetStringContent<T>(T o)
        {
            return new StringContent(ToJson(o), System.Text.Encoding.UTF8, "application/json");
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

        public async Task HubSend<T1, T2, T3, T4>(string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            await HubConnection.SendAsync(methodName, arg1, arg2, arg3, arg4);
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

        public void UnbindHub<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
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

        public void BindHub<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        [Obsolete("Still under construction", true)]
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

        [Obsolete("Still under construction", true)]
        public async Task<Result> CallHubAsync<Result, TPack>(IAbaPackage<TPack> package, [CallerMemberName] string methodName = null)
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