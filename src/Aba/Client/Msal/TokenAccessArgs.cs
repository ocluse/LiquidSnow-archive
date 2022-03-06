using Microsoft.Identity.Client;
using System.Collections.Generic;

namespace Thismaker.Aba.Client.Msal
{
    /// <summary>
    /// Arguments used to obtain an access token
    /// </summary>
    public class TokenAccessArgs
    {
        /// <summary>
        /// Gets or sets the account for which token is to obtained
        /// </summary>
        public IAccount UserAccount { get; set; }

        /// <summary>
        /// The scopes to include when requesting for the tokens
        /// </summary>
        public List<string> Scopes { get; set; }
    }
}
