using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client.Clients
{
    public class AbaClientBuilder<T> where T: ClientBase<T>, new()
    {
        private readonly T client;

        public AbaClientBuilder()
        {
            client = new T();
        }

        /// <summary>
        /// Automatically configure the client
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public AbaClientBuilder<T> WithConfiguration(IConfiguration config)
        {
            client.Configure(config);

            return this;
        }

        /// <summary>
        /// Set the version number of the client
        /// </summary>
        /// <param name="Version"></param>
        /// <returns></returns>
        public AbaClientBuilder<T> WithVersion(string version)
        {
            client.Version = version;

            return this;
        }

        public AbaClientBuilder<T> WithApiEndpoint(string endpoint)
        {
            client.ApiEndpoint = endpoint;

            return this;
        }

        /// <summary>
        /// Provide the context of the client
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public AbaClientBuilder<T> WithContext(IContext context)
        {
            client.Context = context;
            return this;
        }

        public T Build()
        {
            return client;
        }

    }
}
