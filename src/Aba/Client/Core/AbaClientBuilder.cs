using System.Runtime.CompilerServices;
using Thismaker.Aba.Client.Core;

[assembly: InternalsVisibleTo("Thismaker.Aba.Client.Configuration")]
namespace Thismaker.Aba.Client.Core
{
    /// <summary>
    /// Builder to help you quickly create an Aba-based Client application.
    /// Can be used to preconfigure the client and make client singleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbaClientBuilder<T> where T: CoreClient<T>, new()
    {
        internal readonly T client;

        public AbaClientBuilder()
        {
            client = new T();
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

        /// <summary>
        /// Makes the client the singleton client, allowing for app-wide reference
        /// </summary>
        /// <returns></returns>
        public AbaClientBuilder<T> AsSingleton()
        {
            client.MakeSingleton();
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

        /// <summary>
        /// Makes the app by calling <see cref="ClientBase{TClient}.MakeApp"/>
        /// </summary>
        /// <returns></returns>
        public AbaClientBuilder<T> Make()
        {
            client.MakeApp();
            return this;
        }

        /// <summary>
        /// Returns the client App built by this builder
        /// </summary>
        /// <returns></returns>
        public T Build()
        {
            return client;
        }
    }
}
