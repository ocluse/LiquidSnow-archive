using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Aba.Client
{
    /// <summary>
    /// The base class client with utility methods for sending HTTP requests to server and managing client authorization.  
    /// </summary>
    /// <typeparam name="TClient">The type of the clent</typeparam>
    public abstract partial class ClientBase<TClient> where TClient : ClientBase<TClient>
    {
        private class RenewAccessNotImplementedException : Exception { }
        private enum HttpVerb
        {
            Get, Post, Put, Delete
        }

        #region Properties
        /// <summary>
        /// The context of the client.
        /// </summary>
        /// <remarks>
        /// This is usually platform specific, and is useful where there are plaftform specific
        /// method implementations required, for example, when defining app data paths.
        /// </remarks>
        public IContext Context { get; private set; }

        /// <summary>
        /// The singleton instance of the client
        /// </summary>
        public static TClient Instance { get; private set; }

        /// <summary>
        /// Represents the base url address to the server api endpoint e.g www.liquidsnow.com
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// The default endpoint preppended to every request made using the Api HTTP Verbs.
        /// If left as null, the endpoint must be included in the request uri
        /// </summary>
        public string ApiEndpoint { get; set; }

        /// <summary>
        /// The access token that allows the client to access a protected resource
        /// </summary>
        public AccessToken AccessToken { get; set; }

        /// <summary>
        /// The client used to make Http Requests
        /// </summary>
        protected HttpClient HttpClient { get; set; }

        #endregion

        #region Abstract Methods
        /// <summary>
        /// Called by the HTTP helpers to deserialize a response. Override to customize how deserialization works
        /// </summary>
        /// <typeparam name="T">The type of required result</typeparam>
        /// <param name="args">The string to deserialize</param>
        protected abstract T Deserialize<T>(string args);

        /// <summary>
        /// Called by the HTTP helpers to serialize Http string content
        /// </summary>
        /// <typeparam name="T">The type of object being serialized</typeparam>
        /// <param name="args">The object to serialize</param>
        protected abstract string Serialize<T>(T args);
        #endregion

        #region Initialization
        /// <summary>
        /// Makes the app the singleton that can be easily accessed by <see cref="ClientBase{T}.Instance"/>
        /// </summary>
        /// <remarks>
        /// This is useful where only one Client App is needed accross the entire application
        /// </remarks>
        public void MakeSingleton()
        {
            Instance = (TClient)this;
        }

        /// <summary>
        /// Initializes the HttpClient. Should be called only once during the lifetime 
        /// of the client app. Overrride to customize how the client is made
        /// </summary>
        public virtual void MakeApp()
        {
            HttpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called whenever the <see cref="AccessToken"/> has expired or is null.
        /// </summary>
        protected virtual Task RenewAccessTokenAsync()
        {
            throw new RenewAccessNotImplementedException();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the <see cref="Context"/>
        /// </summary>
        public virtual void SetContext(IContext context)
        {
            Context = context;
        }

        #endregion

        #region Http Methods

        /// <summary>
        /// Adds the required Authorization headers to the <paramref name="requestMessage"/>
        /// </summary>
        /// <remarks>
        /// Overriding allows the addition of other specific headers to the request message.
        /// The method will check if the <see cref="AccessToken"/> is currently valid.
        /// </remarks>
        /// <param name="requestMessage">The request message to add Authorization headers to</param>
        /// <param name="isProtected">If true, the authorization headers will be added depending on the <see cref="AccessToken"/> information</param>
        /// <exception cref="ExpiredTokenException"></exception>
        protected virtual async Task PrepareRequestAsync(HttpRequestMessage requestMessage, bool isProtected)
        {
            if (isProtected)
            {
                if (AccessToken == null || AccessToken.IsExpired())
                {
                    try
                    {
                        await RenewAccessTokenAsync();
                    }
                    catch(RenewAccessNotImplementedException)
                    {
                        throw new ExpiredTokenException();
                    }
                }

                if (AccessToken.HeaderName == "Authorization")
                {
                    requestMessage.Headers.Authorization
                        = AccessToken.Value == null ?
                        new AuthenticationHeaderValue(AccessToken.Scheme) :
                        new AuthenticationHeaderValue(AccessToken.Scheme, AccessToken.Value);
                }
                else
                {
                    requestMessage.Headers.Add(AccessToken.HeaderName, AccessToken.Value);
                }
            }
        }

        /// <summary>
        /// Seends a HTTP request to the server base address.
        /// </summary>
        /// <param name="requestMessage">The HTTP request to send</param>
        /// <param name="isProtected">If true, authorization header is added automatically</param>
        /// <param name="option">The value specifying when the response should be determined as complete</param>
        /// <param name="cancellationToken">The token to monitor for cancelling the request</param>
        /// <returns>The response received from the server</returns>
        public async Task<HttpResponseMessage> HttpSendAsync(HttpRequestMessage requestMessage, bool isProtected, HttpCompletionOption option = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            await PrepareRequestAsync(requestMessage, isProtected);
            return await HttpClient.SendAsync(requestMessage, option, cancellationToken);
        }

        /// <summary>
        /// Sends a HTTP GET to the API endpoint of the server
        /// </summary>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <returns>The response received from the server</returns>
        public async Task<HttpResponseMessage> ApiGetAsync(string requestUri, bool isProtected = true)
        {
            return await EndpointGetAsync(ApiEndpoint, requestUri, isProtected);
        }

        /// <summary>
        /// Sends a HTTP POST to the API endpoint of the server
        /// </summary>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="content">The body of the request</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <returns>The response received from the server</returns>
        public async Task<HttpResponseMessage> ApiPostAsync(string requestUri, HttpContent content, bool isProtected = true)
        {
            return await EndpointPostAsync(ApiEndpoint, requestUri, content, isProtected);
        }

        /// <summary>
        /// Sends a HTTP PUT to the API endpoint of the server
        /// </summary>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="content">The body of the request</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <returns>The response received from the server</returns>
        public async Task<HttpResponseMessage> ApiPutAsync(string requestUri, HttpContent content, bool isProtected = true)
        {
            return await EndpointPutAsync(ApiEndpoint, requestUri, content, isProtected);
        }

        /// <summary>
        /// Sends a HTTP DELETE to the API endpoint of the server
        /// </summary>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <returns>The response received from the server</returns>
        public async Task<HttpResponseMessage> ApiDeleteAsync(string requestUri, bool isProtected = true)
        {
            return await EndpointDeleteAsync(ApiEndpoint, requestUri, isProtected);
        }

        /// <summary>
        /// Sends a HTTP GET to the specified endpoint of the server
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request to</param>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <returns>The response received from the server</returns>
        public async Task<HttpResponseMessage> EndpointGetAsync(string endpoint, string requestUri, bool isProtected = true)
        {
            return await ExecuteVerbAsync(HttpVerb.Get, endpoint, requestUri, null, isProtected);
        }

        /// <summary>
        /// Sends a HTTP POST to the specified endpoint of the server
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request to</param>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="content">The body of the request</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <returns>The response received from the server</returns>
        public async Task<HttpResponseMessage> EndpointPostAsync(string endpoint, string requestUri, HttpContent content, bool isProtected = true)
        {
            return await ExecuteVerbAsync(HttpVerb.Post, endpoint, requestUri, content, isProtected);
        }

        /// <summary>
        /// Sends a HTTP PUT to the specified endpoint of the server
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request to</param>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="content">The body of the request</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <returns>The response received from the server</returns>
        public async Task<HttpResponseMessage> EndpointPutAsync(string endpoint, string requestUri, HttpContent content, bool isProtected = true)
        {
            return await ExecuteVerbAsync(HttpVerb.Put, endpoint, requestUri, content, isProtected);
        }

        /// <summary>
        /// Sends a HTTP DELETE to the specified endpoint of the server
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request to</param>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <returns>The response received from the server</returns>
        public async Task<HttpResponseMessage> EndpointDeleteAsync(string endpoint, string requestUri, bool isProtected = true)
        {
            return await ExecuteVerbAsync(HttpVerb.Delete, endpoint, requestUri, null, isProtected);
        }

        private async Task<HttpResponseMessage> ExecuteVerbAsync(HttpVerb verb, string endpoint, string requestUri, HttpContent content, bool isProtected = true)
        {
            HttpMethod method = verb switch
            {
                HttpVerb.Get => HttpMethod.Get,
                HttpVerb.Post => HttpMethod.Post,
                HttpVerb.Put => HttpMethod.Put,
                HttpVerb.Delete => HttpMethod.Delete,
                _ => throw new NotImplementedException("Unknown/unimplemented verb")
            };

            Uri uri = string.IsNullOrEmpty(endpoint)
                ? new Uri(requestUri, UriKind.Relative)
                : new Uri($"{endpoint}/{requestUri}", UriKind.Relative);

            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = method,
                Content = content
            };

            return await HttpSendAsync(requestMessage, isProtected);
        }

        #region Simple Methods

        /// <summary>
        /// Sends a HTTP GET to the API endpoint of the server, deserializing the content body of the response
        /// </summary>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <param name="requiredStatusCode">If specified, an exception is thrown if the response status code is different, otherwise, any success status code suffices</param>
        /// <returns>The deserialized content body of the response</returns>
        public async Task<TResult> ApiGetSimpleAsync<TResult>(string requestUri, bool isProtected = true, HttpStatusCode? requiredStatusCode = null)
        {
            return await EndpointGetSimpleAsync<TResult>(ApiEndpoint, requestUri, isProtected, requiredStatusCode);
        }

        /// <summary>
        /// Sends a HTTP POST to the API endpoint of the server, deserializing the content body of the response
        /// </summary>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="content">The content body of the request</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <param name="serializeContent">If true, the content will be serialized and added as string content to the HTTP request, otherwise it is treated as HttpContent</param>
        /// <param name="deserializeResult">If true, the HTTP response content body will be deserialized, otherwise, the method returns the ressponse message</param>
        /// <param name="requiredStatusCode">If specified, an exception is thrown if the response status code is different, otherwise, any success status code suffices</param>
        /// <returns>The deserialized content body of the response</returns>
        public async Task<TResult> ApiPostSimpleAsync<TContent, TResult>(string requestUri, TContent content, bool isProtected = true, bool serializeContent = true, bool deserializeResult = true, HttpStatusCode? requiredStatusCode = null)
        {
            return await EndpointPostSimpleAsync<TContent, TResult>(ApiEndpoint, requestUri, content, isProtected, serializeContent, deserializeResult, requiredStatusCode);
        }

        /// <summary>
        /// Sends a HTTP PUT to the API endpoint of the server, deserializing the content body of the response
        /// </summary>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="content">The content body of the request</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <param name="serializeContent">If true, the content will be serialized and added as string content to the HTTP request, otherwise it is treated as HttpContent</param>
        /// <param name="deserializeResult">If true, the HTTP response content body will be deserialized, otherwise, the method returns the ressponse message</param>
        /// <param name="requiredStatusCode">If specified, an exception is thrown if the response status code is different, otherwise, any success status code suffices</param>
        /// <returns>The deserialized content body of the response</returns>
        public async Task<TResult> ApiPutSimpleAsync<TContent, TResult>(string requestUri, TContent content, bool isProtected = true, bool serializeContent = true, bool deserializeResult = true, HttpStatusCode? requiredStatusCode = null)
        {
            return await EndpointPutSimpleAsync<TContent, TResult>(ApiEndpoint, requestUri, content, isProtected, serializeContent, deserializeResult, requiredStatusCode);
        }

        /// <summary>
        /// Sends a HTTP DELETE to the API endpoint of the server, deserializing the content body of the response
        /// </summary>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <param name="requiredStatusCode">If specified, an exception is thrown if the response status code is different, otherwise, any success status code suffices</param>
        /// <returns>The deserialized content body of the response</returns>
        public async Task<TResult> ApiDeleteSimpleAsync<TResult>(string requestUri, bool isProtected = true, HttpStatusCode? requiredStatusCode = null)
        {
            return await EndpointDeleteSimpleAsync<TResult>(ApiEndpoint, requestUri, isProtected, requiredStatusCode);
        }

        /// <summary>
        /// Sends a HTTP GET to the specified endpoint of the server, deserializing the content body of the response
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request to</param>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <param name="requiredStatusCode">If specified, an exception is thrown if the response status code is different, otherwise, any success status code suffices</param>
        /// <returns>The deserialized content body of the response</returns>
        public async Task<TResult> EndpointGetSimpleAsync<TResult>(string endpoint, string requestUri, bool isProtected = true, HttpStatusCode? requiredStatusCode = null)
        {
            return await ExecuteVerbSimpleAsync<bool, TResult>(HttpVerb.Get, endpoint, requestUri, false, isProtected, requiredStatusCode: requiredStatusCode);
        }

        /// <summary>
        /// Sends a HTTP POST to the specified endpoint of the server, deserializing the content body of the response
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request to</param>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="content">The content body of the request</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <param name="serializeContent">If true, the content will be serialized and added as string content to the HTTP request, otherwise it is treated as HttpContent</param>
        /// <param name="deserializeResult">If true, the HTTP response content body will be deserialized, otherwise, the method returns the ressponse message</param>
        /// <param name="requiredStatusCode">If specified, an exception is thrown if the response status code is different, otherwise, any success status code suffices</param>
        /// <returns>The deserialized content body of the response</returns>
        public async Task<TResult> EndpointPostSimpleAsync<TContent, TResult>(string endpoint, string requestUri, TContent content, bool isProtected = true, bool serializeContent = true, bool deserializeResult = true, HttpStatusCode? requiredStatusCode = null)
        {
            return await ExecuteVerbSimpleAsync<TContent, TResult>(HttpVerb.Post, endpoint, requestUri, content, isProtected, serializeContent, deserializeResult, requiredStatusCode);
        }

        /// <summary>
        /// Sends a HTTP PUT to the specified endpoint of the server, deserializing the content body of the response
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request to</param>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="content">The content body of the request</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <param name="serializeContent">If true, the content will be serialized and added as string content to the HTTP request, otherwise it is treated as HttpContent</param>
        /// <param name="deserializeResult">If true, the HTTP response content body will be deserialized, otherwise, the method returns the ressponse message</param>
        /// <param name="requiredStatusCode">If specified, an exception is thrown if the response status code is different, otherwise, any success status code suffices</param>
        /// <returns>The deserialized content body of the response</returns>
        public async Task<TResult> EndpointPutSimpleAsync<TContent, TResult>(string endpoint, string requestUri, TContent content, bool isProtected = true, bool serializeContent = true, bool deserializeResult = true, HttpStatusCode? requiredStatusCode = null)
        {
            return await ExecuteVerbSimpleAsync<TContent, TResult>(HttpVerb.Put, endpoint, requestUri, content, isProtected, serializeContent, deserializeResult, requiredStatusCode);
        }

        /// <summary>
        /// Sends a HTTP DELETE to the specified endpoint of the server, deserializing the content body of the response
        /// </summary>
        /// <param name="endpoint">The endpoint to send the request to</param>
        /// <param name="requestUri">The relative URI of the resource</param>
        /// <param name="isProtected">Whether authorization is required to access the resource</param>
        /// <param name="requiredStatusCode">If specified, an exception is thrown if the response status code is different, otherwise, any success status code suffices</param>
        /// <returns>The deserialized content body of the response</returns>
        public async Task<TResult> EndpointDeleteSimpleAsync<TResult>(string endpoint, string requestUri, bool isProtected = true, HttpStatusCode? requiredStatusCode = null)
        {
            return await ExecuteVerbSimpleAsync<bool, TResult>(HttpVerb.Delete, endpoint, requestUri, false, isProtected, requiredStatusCode: requiredStatusCode);
        }

        private bool RequiresContent(HttpVerb verb)
        {
            return verb == HttpVerb.Put || verb == HttpVerb.Post;
        }

        private async Task<TResult> ExecuteVerbSimpleAsync<TContent, TResult>(HttpVerb verb, string endpoint, string requestUri, TContent content, bool isProtected, bool serializeContent = true, bool deserializeResult = true, HttpStatusCode? requiredStatusCode = null)
        {
            bool requiresContent = RequiresContent(verb);

            HttpContent finalContent = null;

            if (requiresContent)
            {
                // Content validation:
                if (!serializeContent)
                {
                    if (!typeof(HttpContent).IsAssignableFrom(typeof(TContent)))
                    {
                        throw new ArgumentException($"{nameof(TContent)} provided must be of type/derive from {nameof(HttpContent)} if {nameof(serializeContent)} is set to false", nameof(serializeContent));
                    }
                }

                //Result validation
                if (!deserializeResult)
                {
                    if (!typeof(HttpResponseMessage).IsAssignableFrom(typeof(TResult)))
                    {
                        throw new ArgumentException($"Generic {nameof(TResult)} provided must be of type or base of {nameof(HttpResponseMessage)} if {nameof(deserializeResult)} is set to false", nameof(serializeContent));
                    }
                }

                finalContent = serializeContent ?
                    GetStringContent(content) : (HttpContent)(object)content;
            }


            HttpResponseMessage response = await ExecuteVerbAsync(verb, endpoint, requestUri, finalContent, isProtected);


            if (!deserializeResult)
            {
                return (TResult)(object)response;
            }

            if ((requiredStatusCode.HasValue && response.StatusCode != requiredStatusCode) ||
                (!requiredStatusCode.HasValue && !response.IsSuccessStatusCode))
            {
                throw new SimpleRequestException(response);
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            return Deserialize<TResult>(responseContent);
        }

        #endregion

        #region Helpers


        /// <summary>
        /// Called by the HTTP POST helper to create a string content out of a provided object. Override to customize the behaviour.
        /// By default, the string content is encoded using UTF8 encoding with a mediatype of "application/json"
        /// </summary>
        protected virtual StringContent GetStringContent<T>(T o)
        {
            return new StringContent(Serialize(o), System.Text.Encoding.UTF8, "application/json");
        }

        #endregion

        #endregion
    }
}
