using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

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
        /// Initializes the HttpClient and the HubConnection. Should be called only onces during the lifetime of the application.
        /// This method must be called before accessing any of the Hub and HttpClient methods.
        /// Overriding this method allows you to specify how the HttpClient and HubConnection should be initialized.
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
        /// Can be used for persistance accross the applications's lifetime.
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
        /// Prepares the HttpClient by setting the appropriate request header, such as Bearer token, before sending the HTTP request.
        /// This method is always run before all HTTP API/Endpoint Methods such as <see cref="ApiGetAsync(string, bool)"/>
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
        /// Sends a HTTP GET request to the Api Endpoint.
        /// </summary>
        /// <param name="requestUri">The specific request uri of the resource</param>
        /// <param name="secured">If true, ensures that the <see cref="AccessToken"/> is added to the request headers</param>
        /// <returns>The <see cref="HttpResponseMessage"/> returned</returns>
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
        /// A simpler way to send a HTTP GET. The result, if successful is deserialized through <see cref="Deserialize{TResult}(string)"/>
        /// If the response does not indicate success, a <see cref="ClientException"/> with be thrown.
        /// </summary>
        /// <typeparam name="TResult">The type to deserialize the content to</typeparam>
        /// <param name="requestUri">The specific resource Uri on the endpoint</param>
        /// <param name="secured">If true, ensures that the method adds the <see cref="AccessToken"/> to the request for a secured resource</param>
        /// <returns>The deserialised content/body of the response received</returns>
        /// <exception cref="ClientException"/>
        public async Task<TResult>  ApiGetSimpleAsync<TResult>(string requestUri, bool secured = true)
        {
            try
            {
                var response = await ApiGetAsync(requestUri, secured);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return Deserialize<TResult>(json);
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

        /// <summary>
        /// Sends a HTTP POST request to the Api Endpoint
        /// </summary>
        /// <param name="requestUri">The specific uri of the request</param>
        /// <param name="content">The content to send to in the body of the request</param>
        /// <param name="secured">If true, ensures that the <see cref="AccessToken"/> is added to the request headers</param>
        /// <returns>The response received from the HTTP request</returns>
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
        /// A simpler way to send a HTTP POST request to the Api endpoint
        /// </summary>
        /// <typeparam name="TContent">The type of content to be added to the body of the request<paramref name="content"/></typeparam>
        /// <typeparam name="TResult">The type of the expected result from the method</typeparam>
        /// <param name="requestUri">The specific uri of the post resource</param>
        /// <param name="content">The content to be included in the body of the request</param>
        /// <param name="secured">If true, ensures that an Authorization header, 
        /// or custom auth header is added depending on the <see cref="AccessToken"/></param>
        /// <param name="serializeContent">If true, the content is serialized first by use of the 
        /// <see cref="GetStringContent{T}(T)"/> method. Otherwise, the content <b>must</b> be of type <see cref="HttpContent"/></param>
        /// <param name="deserializeResult">If true, the method first deserializes the Content
        /// (assumes it to be JSON unless <see cref="Deserialize{T}(string)"/> has been overriden)  of the <b>response</b> message</param>
        /// <returns>A <see cref="HttpResponseMessage"/> or generic type depending on the params</returns>
        /// <exception cref="ClientException">When <paramref name="deserializeResult"/> is true and the request failed</exception>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="InvalidOperationException"/>
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
                    return Deserialize<TResult>(json);
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


        /// <summary>
        /// Similar to <see cref="ApiGetAsync(string, bool)"/> but instead sends the request to the provided endpoint.
        /// </summary>
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

        /// <summary>
        /// Similar to <see cref="ApiGetSimpleAsync(string, bool)"/> but instead sends the request to the provided endpoint.
        /// </summary>
        public async Task<TResult> EndpointGetSimpleAsync<TResult>(string endpoint, string requestUri, bool secured = true)
        {
            try
            {
                var response = await EndpointGetAsync(endpoint, requestUri, secured);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return Deserialize<TResult>(json);
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
        /// Similar to <see cref="ApiPostAsync(string, HttpContent, bool)"/> but instead sends the request to the provided endpoint.
        /// </summary>
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

        /// <summary>
        /// Similar to <see cref="ApiPostSimpleAsync{TContent, TResult}(string, TContent, bool, bool, bool)"/> but instead sends the request to the provided endpoint.
        /// </summary>
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
                    return Deserialize<TResult>(json);
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
        /// <summary>
        /// Called by the HTTP helpers to deserialize a response. Override to customize how deserialization works
        /// </summary>
        /// <typeparam name="T">The type of required result</typeparam>
        /// <param name="args">The string to deserialize</param>
        protected virtual T Deserialize<T>(string args)
        {
            return JsonSerializer.Deserialize<T>(args, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// Called by the HTTP helpers, particularly the POST helper to serialize an object to be added to the body of the request
        /// </summary>
        /// <typeparam name="T">The type of object being serialized</typeparam>
        /// <param name="args">The object to serialize</param>
        protected virtual string Serialize<T>(T args)
        {
            return JsonSerializer.Serialize(args);
        }

        /// <summary>
        /// Called by the HTTP POST helper to create a string content out of a provided object. Override to customize the behaviour.
        /// By default, the string content is encoded using UTF8 encoding with a mediatype of "application/json"
        /// </summary>
        protected virtual StringContent GetStringContent<T>(T o)
        {
            return new StringContent(Serialize(o), System.Text.Encoding.UTF8, "application/json");
        }

        #endregion

        #region Hub
        /// <summary>
        /// When overriden, allows the client to subscribe methods to the Hub
        /// </summary>
        public abstract void SubscribeHub();

        /// <summary>
        /// When overriden, allows the client to unsubscribe methods from the Hub
        /// </summary>
        public abstract void UnsubscribeHub();

        /// <summary>
        /// Attempts to connect to the hub. Should be called only once during the lifetime of the application,
        /// unless the client was disconnected and they wish to reconnect.
        /// Calling this method when the hub was already online throws an exception.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectHub()
        {
            try
            {
                await HubConnection.StartAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Send an RPC to the connected hub.
        /// </summary>
        public async Task HubSend(string methodName)
        {
            await HubConnection.SendAsync(methodName);
        }

        /// <summary>
        /// Send an RPC to the connected hub.
        /// </summary>
        public async Task HubSend(string methodName, object arg1)
        {
            await HubConnection.SendAsync(methodName, arg1);
        }

        /// <summary>
        /// Send an RPC to the connected hub.
        /// </summary>
        public async Task HubSend(string methodName, object arg1, object arg2)
        {
            await HubConnection.SendAsync(methodName, arg1, arg2);
        }

        /// <summary>
        /// Send an RPC to the connected hub.
        /// </summary>
        public async Task HubSend(string methodName, object arg1, object arg2, object arg3)
        {
            await HubConnection.SendAsync(methodName, arg1, arg2, arg3);
        }

        /// <summary>
        /// Send an RPC to the connected hub. If you wish to send more than four arguments, consider using <see cref="HubConnection"/> directly
        /// </summary>
        public async Task HubSend(string methodName, object arg1, object arg2, object arg3, object arg4)
        {
            await HubConnection.SendAsync(methodName, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Unsubscribes a method from the hub
        /// </summary>
        public void UnbindHub(Action action)
        {
            var name = action.Method.Name;
            HubConnection.Remove(name);
        }

        /// <summary>
        /// Unsubscribes a method from the hub
        /// </summary>
        public void UnbindHub<T1>(Action<T1> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        /// <summary>
        /// Unsubscribes a method from the hub
        /// </summary>
        public void UnbindHub<T1,T2>(Action<T1,T2> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        /// <summary>
        /// Unsubscribes a method from the hub
        /// </summary>
        public void UnbindHub<T1,T2,T3>(Action<T1,T2,T3> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        /// <summary>
        /// Unsubscribes a method from the hub
        /// </summary>
        public void UnbindHub<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        /// <summary>
        /// Adds a method to be called when the Hub recieves an RPC of the same name as the method
        /// </summary>
        public void BindHub(Action action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        /// <summary>
        /// Adds a method to be called when the Hub recieves an RPC of the same name as the method
        /// </summary>
        public void BindHub<T1>(Action<T1> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        /// <summary>
        /// Adds a method to be called when the Hub recieves an RPC of the same name as the method
        /// </summary>
        public void BindHub<T1,T2>(Action<T1,T2> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        /// <summary>
        /// Adds a method to be called when the Hub recieves an RPC of the same name as the method
        /// </summary>
        public void BindHub<T1,T2,T3>(Action<T1,T2,T3> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
        }

        /// <summary>
        /// Adds a method to be called when the Hub recieves an RPC of the same name as the method
        /// </summary>
        public void BindHub<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        {
            var name = action.Method.Name;
            HubConnection.On(name, action);
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
        public async Task<TResult> CallHubAsync<TResult>(string requestName, string responseName=null, object content=null, double? timeOut=null)
        {
            if (string.IsNullOrEmpty(responseName))
            {
                responseName = requestName;
            }
            var tcs = new TaskCompletionSource<TResult>();

            if (timeOut.HasValue)
            {
                var timer = new System.Timers.Timer(timeOut.Value)
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
            HubConnection.On<TResult>(responseName, OnHubResponse);
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