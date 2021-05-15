using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Mercury
{
    static class Globals
    {
        private static byte[] _disconnectBytes, _disconnectAckBytes, _pingBytes, _pingAckBytes;

        public static byte[] Disconnect
        {
            get
            {
                if (_disconnectBytes == null)
                {
                    _disconnectBytes = "____DISCONCT".GetBytes<ASCIIEncoding>();
                }

                return _disconnectBytes;
            }
        }

        public static byte[] AckDisconnect
        {
            get
            {
                if (_disconnectAckBytes == null)
                {
                    _disconnectAckBytes = "ACK_DISCONCT".GetBytes<ASCIIEncoding>();
                }

                return _disconnectAckBytes;
            }
        }

        public static byte[] Ping
        {
            get
            {
                if (_pingBytes == null)
                {
                    _pingBytes = "________PING".GetBytes<ASCIIEncoding>();
                }
                return _pingBytes;
            }
        }

        public static byte[] AckPing
        {
            get
            {
                if (_pingAckBytes == null)
                {
                    _pingAckBytes = "____ACK_PING".GetBytes<ASCIIEncoding>();
                }

                return _pingAckBytes;
            }
        }

        public static bool Compare(this byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }
    }
}
