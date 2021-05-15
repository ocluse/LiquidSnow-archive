using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Reflection;
using Thismaker.Aba.Server.Mercury.Hosted;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MercuryDependencyInjectionExtensions
    {
        public static IServiceCollection AddMercury(this IServiceCollection services, HostedManagerOptions options)
        {
            services.AddSingleton(options);
            services.AddHostedService<HostedMercuryManager>();
            return services;
        }

        public static IMvcBuilder AddMercuryAuthenticator(this IMvcBuilder builder)
        {
            builder.AddApplicationPart(Assembly.GetAssembly(typeof(AuthenticatorController)));
            return builder;
        }
    }
}

public class MercuryOptions
{
    public string Address { get; set; }
    public Type ServiceType { get; set; }
    public int Port { get; set; }
}

public class HostedManagerOptions
{
    public string HostAddress { get; set; }
    public List<MercuryOptions> ServerOptions { get; set; }

    public HostedManagerOptions WithAddress(string address)
    {
        HostAddress = address;
        return this;
    }

    public HostedManagerOptions AddServer<TServer>(int port) where TServer : HostedMercuryServer
    {
        return AddServer<TServer>(null, port);
    }

    public HostedManagerOptions AddServer<TServer>(string address, int port)where TServer : HostedMercuryServer
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
