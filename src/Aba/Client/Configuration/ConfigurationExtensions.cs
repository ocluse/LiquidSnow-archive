using Microsoft.Extensions.Configuration;

namespace Thismaker.Aba.Client
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Automatically configure the client, calling the required methods afterwards.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static AbaClientBuilder<T> WithConfiguration<T>(this AbaClientBuilder<T> aba, IConfiguration config, string configKey = "AbaClient", bool make = true) where T:ClientBase<T>, new()
        {
            aba.client.Configure(config, configKey, make);
            return aba;
        }

        /// <summary>
        /// Configures the app using the provided Configuration, calling <see cref="ClientBase{T}.MakeApp"/> once done
        /// </summary>
        public static void Configure<T>(this ClientBase<T> client, IConfiguration config, string configKey="AbaClient", bool make=true) where T : ClientBase<T>
        {
            config.Bind(configKey, client);
            if (make)
            {
                client.MakeApp();
            }         
        }
    }
}
