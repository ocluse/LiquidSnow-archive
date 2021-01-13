using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client
{
    public interface IContext
    {
        void Start();

        object GetMainWindow();

        void Shutdown();
    }
}
