using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Thismaker.Aba.Client.Configuration")]
[assembly: InternalsVisibleTo("Thismaker.Aba.Client.SignalR")]
[assembly: InternalsVisibleTo("Thismaker.Aba.Client.Mercury")]

namespace Thismaker.Aba.Client
{
    /// <summary>
    /// Builder to help you quickly create an Aba-based Client application.
    /// Can be used to preconfigure the client and make client singleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbaClientBuilder<T> where T : ClientBase<T>, new()
    {
        internal readonly T client;
        private bool made = false;

        /// <summary>
        /// Creates a new builder that will be used to build an instance of a client app
        /// </summary>
        public AbaClientBuilder()
        {
            client = new T();
        }

        /// <summary>
        /// Sets the api endpoint of the client
        /// </summary>
        /// <returns></returns>
        public AbaClientBuilder<T> WithApiEndpoint(string apiEndpoint)
        {
            EnsureNotMadeYet();
            client.ApiEndpoint = apiEndpoint;
            return this;
        }

        /// <summary>
        /// Sets the base address of the client
        /// </summary>
        /// <returns></returns>
        public AbaClientBuilder<T> WithBaseAddress(string address)
        {
            EnsureNotMadeYet();
            client.BaseAddress = address;
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
        /// <param name="context">The context to apply to the client</param>
        /// <returns></returns>
        public AbaClientBuilder<T> WithContext(IContext context)
        {
            client.SetContext(context);
            return this;
        }

        /// <summary>
        /// Throws an exception in case the client has already been made
        /// </summary>
        public void EnsureNotMadeYet()
        {
            if (made)
            {
                throw new InvalidOperationException($"Client already made");
            }
        }

        /// <summary>
        /// Throws an exception in case the client has not been made yet
        /// </summary>
        public void EnsureAlreadyMade()
        {
            if (made)
            {
                throw new InvalidOperationException($"Client not made yet");
            }
        }

        /// <summary>
        /// Makes the app by calling <see cref="ClientBase{TClient}.MakeApp"/>
        /// </summary>
        /// <returns></returns>
        public AbaClientBuilder<T> Make()
        {
            client.MakeApp();
            made = true;
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
