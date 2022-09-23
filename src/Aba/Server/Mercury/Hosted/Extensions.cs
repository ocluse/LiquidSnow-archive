using System.Reflection;
using Thismaker.Aba.Server.Mercury.Hosted;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MercuryDependencyInjectionExtensions
    {
        /// <summary>
        /// Adds support for Mercury servers to the project.
        /// </summary>
        public static IServiceCollection AddMercury(this IServiceCollection services, HostedManagerOptions options)
        {
            services.AddSingleton(options);
            services.AddHostedService<HostedMercuryManager>();
            return services;
        }

        /// <summary>
        /// Adds an Authentication Controller that will be specifically used by Mercury to verify AccessTokens.
        /// This must be included if <see cref="Thismaker.Aba.Server.Mercury.RequireTokenValidationAttribute"/> is used.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddMercuryAuthenticator(this IMvcBuilder builder)
        {
            builder.AddApplicationPart(Assembly.GetAssembly(typeof(AuthenticatorController)));
            return builder;
        }
    }
}
