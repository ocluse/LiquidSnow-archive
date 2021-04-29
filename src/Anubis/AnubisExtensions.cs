using System;
using System.Collections;

namespace Thismaker.Anubis
{
    static class AnubisExtensions
    {
        /// <summary>
        /// Returns the byte array as an unsigned long
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static ulong ToULong(this BitArray ba)
        {
            var len = Math.Min(64, ba.Count);
            ulong n = 0;
            for (int i = 0; i < len; i++)
            {
                if (ba.Get(i))
                    n |= 1UL << i;
            }
            return n;
        }

        /// <summary>
        /// Reverses the byteness of the byte array
        /// </summary>
        /// <param name="array"></param>
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

        /// <summary>
        /// Returns the bytes of the byte array as a proper representatins
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this BitArray bits)
        {
            // Make sure we have enough space allocated even when number of bits is not a multiple of 8
            var bytes = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(bytes, 0);
            return bytes;
        }
    }
}
