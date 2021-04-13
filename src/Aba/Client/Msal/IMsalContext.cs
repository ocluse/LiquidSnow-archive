﻿using Microsoft.Identity.Client;
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

        /// <summary>
        /// A handle to allow serialization(storage) of access tokens depending on the executing platform
        /// </summary>
        /// <param name="cache"></param>
        void EnableSerialization(ITokenCache cache);
    }
}
