using Thismaker.Aba.Client.Core;

namespace Thismaker.Aba.Client.Mercury
{
    public static class AbaClientBuilderExtensions
    {
        /// <summary>
        /// Sets the HubEndpoint of a client
        /// </summary>
        /// <returns></returns>
        public static AbaClientBuilder<T> WithHubEndpoint<T>(this AbaClientBuilder<T> aba, string hubEndpoint)where T : ClientBase<T>, new()
        {
            aba.EnsureNotMadeYet();
            aba.client.HubEndpoint = hubEndpoint;
            return aba;
        }

        /// <summary>
        /// Calls <see cref="ClientBase{TClient}.SubscribeHub"/>
        /// </summary>
        /// <returns></returns>
        public static AbaClientBuilder<T> SubscribeHub<T>(this AbaClientBuilder<T> aba) where T : MercuryClient<T>, new()
        {
            aba.EnsureAlreadyMade();
            aba.client.SubscribeHub();
            return aba;
        }
    }
}
