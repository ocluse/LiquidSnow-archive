using System;

namespace Thismaker.Enigma.Classical
{
    public class EMWheel : ICloneable
    {

        public Alphabet Wiring { get; set; }

        public EMWheel() { }

        public EMWheel(Alphabet wiring)
        {
            Wiring = wiring;
        }

        public EMWheel(string wiring)
        {
            Wiring = new Alphabet(wiring);
        }

        public virtual int GetPath(Alphabet source, int pin, bool forward)
        {
            //get the character of the pin:
            if (forward)
            {
                return source.IndexOf(Wiring[pin]);
            }
            else
            {
                return Wiring.IndexOf(source[pin]);
            }
            
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
