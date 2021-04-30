namespace Thismaker.Aba.Client.Core
{
    /// <summary>
    /// The context of the client. Should be impelemeted as e.g WindowsClient, AndroidClient, GeneralClient etc.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Starts the context. Usually called by the derived class
        /// </summary>
        void Start();

        /// <summary>
        /// Called by the derived class so that any leftover cleanup can be made in the impelemnting class
        /// </summary>
        void Shutdown();
    }
}
