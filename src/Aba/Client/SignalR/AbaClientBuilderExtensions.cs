using Thismaker.Aba.Client.SignalR;

namespace Thismaker.Aba.Client
{
    /// <summary>
    /// Extensions for the <see cref="AbaClientBuilder{T}"/>
    /// </summary>
    public static class AbaClientBuilderExtensions
    {
        /// <summary>
        /// Sets the HubEndpoint of a client
        /// </summary>
        /// <returns></returns>
        public static AbaClientBuilder<T> WithHubEndpoint<T>(this AbaClientBuilder<T> aba, string hubEndpoint) where T : SignalRClientBase<T>, new()
        {
            aba.EnsureNotMadeYet();
            aba.client.HubEndpoint = hubEndpoint;
            return aba;
        }

        /// <summary>
        /// Calls <see cref="SignalRClientBase{TClient}.SubscribeHub"/>
        /// </summary>
        /// <returns></returns>
        public static AbaClientBuilder<T> SubscribeHub<T>(this AbaClientBuilder<T> aba) where T : SignalRClientBase<T>, new()
        {
            aba.EnsureAlreadyMade();
            aba.client.SubscribeHub();
            return aba;
        }
    }
}
