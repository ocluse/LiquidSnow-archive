using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Common.Mercury
{
    internal class RPCPayload
    {
        public string MethodName { get; set; }

        public string AccessToken { get; set; }

        public List<string> Parameters { get; set; }
    }
}
