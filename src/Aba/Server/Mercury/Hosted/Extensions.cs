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


