using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Anubis
{
    interface IJector<T>
    {
        byte[] InjectBytes(byte[] input, byte[] data);
        byte[] EjectBytes(byte[] input);

        Task<T> InjectAsync(T input, byte[] data);
        Task<byte[]> EjectAsync(T input);

        T Inject(T input, byte[] data);
        byte[] Eject(T input);
    }
}
