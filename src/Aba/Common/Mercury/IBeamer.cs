using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Aba.Common.Mercury
{
    public interface IBeamer
    {
        Task BeamAsync(string methodName);

        Task BeamAsync<T1>(string methodName, T1 arg);

        Task BeamAsync<T1, T2>(string methodName, T1 arg1, T2 arg2);

        Task BeamAsync<T1, T2, T3>(string methodName, T1 arg1, T2 arg2, T3 arg3);

        Task BeamAsync<T1, T2, T3, T4>(string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        Task BeamAsync(string methodName, object[] args, Type[] types);
    }
}
