using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Aretha
{
    internal interface ISoul
    {
        Soul Soul { get;}
        void Summon(string[] args);
        void Speak(string text);
        string Listen(string question, bool isYN);
        string GetPath(string question, bool input, bool confirm);
    }
}
