using Microsoft.Identity.Client;
using System.Collections.Generic;

namespace Thismaker.Aba.Client.Msal
{
    public class TokenAccessArgs
    {
        public IAccount UserAccount { get; set; }

        public List<string> Scopes { get; set; }
    }
}
