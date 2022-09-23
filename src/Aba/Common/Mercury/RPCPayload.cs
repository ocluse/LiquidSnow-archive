using System.Collections.Generic;

namespace Thismaker.Aba.Common.Mercury
{
    internal class RPCPayload
    {
        public string MethodName { get; set; }

        public string AccessToken { get; set; }

        public List<string> Parameters { get; set; }
    }
}
