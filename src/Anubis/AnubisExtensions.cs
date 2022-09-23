using System;
using System.Collections;

namespace Thismaker.Anubis
{
    internal static class AnubisExtensions
    {
        public static ulong ToULong(this BitArray ba)
        {
            int len = Math.Min(64, ba.Count);
            ulong n = 0;
            for (int i = 0; i < len; i++)
            {
                if (ba.Get(i))
                    n |= 1UL << i;
            }
            return n;
        }

        public static void Reverse(this BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }

        public static byte[] ToBytes(this BitArray bits)
        {
            // Make sure we have enough space allocated even when number of bits is not a multiple of 8
            byte[] bytes = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(bytes, 0);
            return bytes;
        }
    }
}
