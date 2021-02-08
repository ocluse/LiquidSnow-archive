using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client.Msal
{
    /// <summary>
    /// A <see cref="IContext"/> that should be used specifically for <see cref="MsalClient{T}"/>
    /// </summary>
    public interface IMsalContext : IContext
    {
        /// <summary>
        /// A handle to get the MainWindow of the class
        /// </summary>
        /// <returns></returns>
        object GetMainWindow();
    }
}
