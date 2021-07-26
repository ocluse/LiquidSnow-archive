using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Aba.Client
{
    /// <summary>
    /// The context of the client. Should be impelemeted as e.g WindowsClient, AndroidClient, GeneralClient etc.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Starts the context
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Stops the context
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Called by the derived class so that any leftover cleanup can be made in the impelemnting class
        /// </summary>
        Task ShutdownAsync();
    }
}
