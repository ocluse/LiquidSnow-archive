using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Enigma.Classical
{
    public class EnigmaMachine : ClassicalAlgorithm
    {
        #region Private Fields
        #endregion

        #region Constructors
        public EnigmaMachine()
        {
            
        }
        #endregion


        #region Properties

        public List<Rotor> Rotors { get; set; }

        public bool AutoReset { get; set; } = false;

        public bool DoubleStep { get; set; } = false;

        public EMWheel Stator { get; set; }

        public EMWheel Reflector { get; set; }

        public int[] RotorConfig
        {
            get
            {
                if (Rotors == null) 
                    throw new NullReferenceException("Machine has no rotors");

                var list = new List<int>();

                foreach(var rotor in Rotors)
                {
                    list.Add(rotor.Rotation);
                }

                return list.ToArray();
            }
        }

        #endregion

        #region Private Methods

        

        private void OnRotorHitNotch(Rotor rotor)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public Methods
        public void AddRotor(Rotor rotor)
        {
            if (Rotors == null) Rotors = new List<Rotor>();

            Rotors.Add(rotor);
            rotor.HitNotch += OnRotorHitNotch;
        }

        public void RemoveRotor(Rotor rotor)
        {
            Rotors.Remove(rotor);
        }

        public void RepositionRotor(Rotor rotor, int index)
        {
            Rotors.RemoveAt(Rotors.IndexOf(rotor));
            Rotors.Insert(index, rotor);
        }

        public void ResetRotors()
        {
            int i = 0;
            foreach (var rotor in Rotors)
            {
                rotor.Rotation = string.IsNullOrEmpty(Key) ? 0 : Alphabet.IndexOf(Key[i]);
                i++;
            }
        }
        #endregion

        #region Logic Methods

        public override string Run(string input, bool forward)
        {
            var output = new StringBuilder();

            foreach(var c in input) 
                output.Append(Run(c));

            if (AutoReset) ResetRotors();

            return output.ToString();
        }

        public virtual char Run(char input)
        {
            //Rotate the FastRotor
            Rotors[0].Rotate(Alphabet);

            //Pass the current to the Stator:
            int index = Stator.GetPath(Alphabet, Alphabet.IndexOf(input), true);

            //Pass the current through successive rotors:
            for(int i = 0; i < Rotors.Count; i++)
            {
                index = Rotors[i].GetPath(Alphabet, index, true);
            }
            
            //Pass the current the Reflector
            index = Reflector.GetPath(Alphabet, index, true);
            
            //Pass through the rotors in reverse:
            for(int i = Rotors.Count-1; i > -1; i--)
            {
                var rotor = Rotors[i];
                index = rotor.GetPath(Alphabet, index, false);
            }

            //Pass the current through the Stator:
            index = Stator.GetPath(Alphabet, index, true);

            return Alphabet[index];
            
        }
        #endregion
    }

    public class EnigmaMachineComponents
    {
        public string ETW { get; set; }
        public List<string> Rotors { get; set; }
        public List<string> UWK { get; set; }
    }
}