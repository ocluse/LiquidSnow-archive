﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Aba.Client.Core
{
    public abstract partial class CoreClient<TClient> where TClient : CoreClient<TClient>
    {

        #region Properties
        public virtual IContext Context { get; private set; }

        /// <summary>
        /// The singleton instance of the client
        /// </summary>
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

        /// <summary>
        /// The access token that is usually added as an authorization header
        /// to the app's <see cref="HubConnection"/> and <see cref="HttpClient"/>.
        /// </summary>
        public AccessToken AccessToken { get; set; }

        /// <summary>
        /// If true, the client auto renews the access token once it expires by calling
        /// <see cref="RenewAccessTokenAsync"/> function;
        /// </summary>
        public bool AutoRenewAccessToken { get; set; }

        /// <summary>
        /// The client used to access the server api
        /// </summary>
        protected HttpClient HttpClient { get; set; }

        #endregion

        #region Abstract Methods
        /// <summary>
        /// When overriden in a derived class, starts the ClientApp, 
        /// allowing for performing basic tasks.
        /// Should be called once <see cref="MakeApp"/> has been called.
        /// </summary>
        public abstract Task StartAsync();

        /// <summary>
        /// Called when the lifetime of the client has ended to cleanup resources
        /// and persist data if need be.
        /// </summary>
        public abstract Task StopAsync();

        /// <summary>
        /// Called whenever the <see cref="AccessToken"/> has expired and the client is
        /// allowed to automatically renew.
        /// </summary>
        /// <returns></returns>
        protected abstract Task RenewAccessTokenAsync();

        /// <summary>
        /// Called by the HTTP helpers to deserialize a response. Override to customize how deserialization works
        /// </summary>
        /// <typeparam name="T">The type of required result</typeparam>
        /// <param name="args">The string to deserialize</param>
        protected abstract T Deserialize<T>(string args);

        /// <summary>
        /// Called by the HTTP helpers, particularly the POST helper to serialize an object to be added to the body of the request
        /// </summary>
        /// <typeparam name="T">The type of object being serialized</typeparam>
        /// <param name="args">The object to serialize</param>
        protected abstract string Serialize<T>(T args);
        #endregion

        #region Initialization

        /// <summary>
        /// Makes the app the singleton that can be easily accessed by <see cref="ClientBase{T}.Instance"/>
        /// Can be used for persistance accross the applications's lifetime.
        /// </summary>
        public void MakeSingleton()
        {
            Instance = (TClient)this;
        }

        /// <summary>
        /// Initializes the HttpClient. Should be called only once during the lifetime 
        /// of the application
        /// </summary>
        public virtual void MakeApp()
        {
            HttpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the underlying context of the client
        /// </summary>
        /// <param name="context"></param>
        public virtual void SetContext(IContext context)
        {
            Context = context;
        }

        #endregion

        #region Api Methods
        /// <summary>
        /// Prepares the HttpClient by setting the appropriate request header, such as Bearer token, before sending the HTTP request.
        /// This method is always run before all HTTP API/Endpoint Methods such as <see cref="ApiGetAsync(string, bool)"/>
        /// </summary>
        /// <param name="secured">If true, the http client sends along an Authorization or other authentication headers, otherwise, it removes them</param>
        /// <returns></returns>
        protected virtual async Task ReadyHttpClientForRequestAsync(bool secured)
        {
            if (secured)
            {
                //has acccess token expired?
                if (AccessToken.IsExpired())
                {
                    //are we allowed to autorenew the token:
                    if (AutoRenewAccessToken)
                    {
                        await RenewAccessTokenAsync();
                    }
                    else
                    {
                        throw new ExpiredTokenException();
                    }
                }
                
                if (AccessToken.Kind == AccessTokenKind.Custom)
                {
                    HttpClient.DefaultRequestHeaders.Remove(AccessToken.Key);
                    HttpClient.DefaultRequestHeaders.Add(AccessToken.Key, AccessToken.Value);
                }
                else
                {
                    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AccessToken.Key, AccessToken.Value);
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
        public async Task<HttpResponseMessage> ApiGetAsync(string requestUri, bool secured = true)
        {
            //Prep the HttpClient
            await ReadyHttpClientForRequestAsync(secured);

            try
            {
                return await HttpClient.GetAsync(ApiEndpoint == null ? $"/{requestUri}"
                    : $"/{ApiEndpoint}/{requestUri}");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// A simpler way to send a HTTP GET. The result, if successful is deserialized through <see cref="Deserialize{TResult}(string)"/>
        /// If the response does not indicate success, a <see cref="HttpRequestException"/> is thrown
        /// </summary>
        /// <typeparam name="TResult">The type to deserialize the content to</typeparam>
        /// <param name="requestUri">The specific resource Uri on the endpoint</param>
        /// <param name="secured">If true, ensures that the method adds the <see cref="AccessToken"/> to the request for a secured resource</param>
        /// <returns>The deserialised content/body of the response received</returns>
        /// <exception cref="HttpRequestException"/>
        public async Task<TResult> ApiGetSimpleAsync<TResult>(string requestUri, bool secured = true)
        {
            try
            {
                var response = await ApiGetAsync(requestUri, secured);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return Deserialize<TResult>(json);
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
            await ReadyHttpClientForRequestAsync(secured);

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
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="InvalidOperationException"/>
        public async Task<TResult> ApiPostSimpleAsync<TContent, TResult>(string requestUri, TContent content, bool secured = true, bool serializeContent = true, bool deserializeResult = true)
        {
            //Request validation:
            if (!serializeContent)
            {
                if (!content.GetType().IsAssignableFrom(typeof(HttpContent)))
                {
                    throw new InvalidOperationException($"Content provided must be of type/derive from {nameof(HttpContent)} if serializeContent is set to false");
                }
            }

            if (!deserializeResult)
            {
                if (!typeof(TResult).IsAssignableFrom(typeof(HttpResponseMessage)))
                {
                    throw new InvalidOperationException($"Generic TResult provided must be of type/derive from {nameof(HttpResponseMessage)} if serializeResult is set to false");
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

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return Deserialize<TResult>(json);
            }
            catch(Exception ex)
            {
                throw new ClientException(ex);
            }
        }

        /// <summary>
        /// Similar to <see cref="ApiGetAsync(string, bool)"/> but instead sends the request to the provided endpoint.
        /// </summary>
        public async Task<HttpResponseMessage> EndpointGetAsync(string endpoint, string requestUri, bool secured = true)
        {
            //Prep the HttpClient
            await ReadyHttpClientForRequestAsync(secured);

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
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return Deserialize<TResult>(json);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Similar to <see cref="ApiPostAsync(string, HttpContent, bool)"/> but instead sends the request to the provided endpoint.
        /// </summary>
        public async Task<HttpResponseMessage> EndpointPostAsync(string endpoint, string requestUri, HttpContent content, bool secured = true)
        {
            //Prep the HttpClient
            await ReadyHttpClientForRequestAsync(secured);

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
                if (!content.GetType().IsAssignableFrom(typeof(HttpContent)))
                {
                    throw new InvalidOperationException($"Content provided must be of type/derive from {nameof(HttpContent)} if serializeContent is set to false");
                }
            }

            if (!deserializeResult)
            {
                if (!typeof(TResult).IsAssignableFrom(typeof(HttpResponseMessage)))
                {
                    throw new InvalidOperationException($"Generic TResult provided must be of type/derive from {nameof(HttpResponseMessage)} if serializeResult is set to false");
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

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return Deserialize<TResult>(json);
            }
            catch
            {
                throw;
            }
        }

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
