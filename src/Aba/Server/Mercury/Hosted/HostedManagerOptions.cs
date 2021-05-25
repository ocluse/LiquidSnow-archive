using System;
using System.Collections.Generic;

namespace Thismaker.Aba.Server.Mercury.Hosted
{
    public class HostedManagerOptions
    {
        public string HostAddress { get; set; }
        public List<MercuryOptions> ServerOptions { get; set; }

        public HostedManagerOptions(string address)
        {
            HostAddress = address;
        }

        public HostedManagerOptions AddServer<TServer>(int port) where TServer : HostedMercuryServer
        {
            return AddServer<TServer>(null, port);
        }

        public HostedManagerOptions AddServer<TServer>(string address, int port) where TServer : HostedMercuryServer
        {
            var option = new MercuryOptions
            {
                Address = address,
                Port = port,
                ServiceType = typeof(TServer)
            };

            if (ServerOptions == null)
            {
                ServerOptions = new List<MercuryOptions>();
            }

            ServerOptions.Add(option);
            return this;
        }
    }

    public class MercuryOptions
    {
        /// <summary>
        /// The address of the server
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// The type of the client
        /// </summary>
        public Type ServiceType { get; set; }
        
        /// <summary>
        /// The port the client is to run on
        /// </summary>
        public int Port { get; set; }
    }
}
