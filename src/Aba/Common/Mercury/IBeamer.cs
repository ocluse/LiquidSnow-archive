using System;
using System.Threading.Tasks;

namespace Thismaker.Aba.Common.Mercury
{
    /// <summary>
    /// Allows the sending of Beams, which are basically remote procedure calls
    /// </summary>
    public interface IBeamer
    {
        /// <summary>
        /// Send a remote procedure call to invoke the provided method
        /// </summary>
        Task BeamAsync(string methodName);

        /// <summary>
        /// Send a remote procedure call to invoke the provided method
        /// </summary>
        Task BeamAsync<T1>(string methodName, T1 arg);

        /// <summary>
        /// Send a remote procedure call to invoke the provided method
        /// </summary>
        Task BeamAsync<T1, T2>(string methodName, T1 arg1, T2 arg2);

        /// <summary>
        /// Send a remote procedure call to invoke the provided method
        /// </summary>
        Task BeamAsync<T1, T2, T3>(string methodName, T1 arg1, T2 arg2, T3 arg3);

        /// <summary>
        /// Send a remote procedure call to invoke the provided method
        /// </summary>
        Task BeamAsync<T1, T2, T3, T4>(string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        /// <summary>
        /// Send a remote procedure call to invoke the provided method
        /// </summary>
        Task BeamAsync(string methodName, object[] args, Type[] types);
    }
}
