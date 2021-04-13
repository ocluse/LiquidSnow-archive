﻿using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client.Msal
{
    public class TokenAccessArgs
    {
        public IAccount UserAccount { get; set; }

        public List<string> Scopes { get; set; }
    }
}
