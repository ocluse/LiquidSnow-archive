using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Enigma.Classical
{
    public class Rotor : EMWheel
    {
        #region Constructors
        public Rotor() : base() { }

        public Rotor(string wiring) : base(wiring) { }

        public Rotor(Alphabet wiring) : base(wiring) { }

        public Rotor(Alphabet wiring, char turnOver) : base(wiring)
        {
            TurnOver = new List<char>() { turnOver };
        }

        public Rotor(string wiring, char turnOver) : base(wiring)
        {
            TurnOver = new List<char>() { turnOver };
        }

        #endregion

        #region Properties

        public event Action<Rotor> HitNotch;

        public List<char> TurnOver { get; set; }

        public int Rotation { get; set; }

        #endregion

        #region Public Methods
        public void Rotate(Alphabet source)
        {
            Rotation = (Rotation + 1) % Wiring.Count;

            if (TurnOver.Contains(source[Rotation]))
            {
                HitNotch?.Invoke(this);
            }
        }

        public override int GetPath(Alphabet source,int pin, bool forward)
        {
            pin = (pin + Rotation) % Wiring.Count;
            return base.GetPath(source,pin, forward);
        }
        #endregion

    }
}
