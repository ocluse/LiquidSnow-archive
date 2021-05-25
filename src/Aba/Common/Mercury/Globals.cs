using System.Text;

namespace Thismaker.Aba.Common.Mercury
{
    public static class Globals
    {
        private static byte[] _authSelf;

        public static byte[] AuthSelf
        {
            get
            {
                if (_authSelf == null)
                {
                    _authSelf = "___AUTH_SELF".GetBytes<ASCIIEncoding>();
                }

                return _authSelf;
            }
        }

        public const string AuthResponsePayload = "AUTH_PAYLOAD";

        public const string CloseConnection = "__CLOSE_CONN";
    }
}
