using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Thismaker.Horus.Classical
{
    /// <summary>
    /// Represents an Enigma Machine, true to the physical device.
    /// The Enigma was famously used by the German's in WWII to send secret messages.
    /// Due to the enormous size of the mathematical probabilities (in the quitntillions or sth)
    /// the German's errenously believed that it was <b>unbreakable</b>
    /// This is what inspired me to create the entire Enigma library, and by extension Liquid Snow.
    /// So glad that this thing actually works, like the real machine :) Enjoy
    /// </summary>
    public partial class EnigmaMachine : ClassicalAlgorithm
    {
        #region Private Fields
        #endregion

        #region Constructors
        

        public EnigmaMachine()
        {
            Rotors.CollectionChanged += Rotors_CollectionChanged;
        }

        public EnigmaMachine(IEnumerable<Rotor> rotors)
        {
            Rotors.CollectionChanged += Rotors_CollectionChanged;

            Rotors.AddRange(rotors);
        }

        private void Rotors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                ||e.Action==NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.NewItems)
                {
                    var rotor = (Rotor)item;
                    rotor.HitNotch += OnRotorHitNotch;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove
                ||e.Action==NotifyCollectionChangedAction.Replace
                ||e.Action==NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in e.OldItems)
                {
                    var rotor = (Rotor)item;
                    rotor.HitNotch -= OnRotorHitNotch;
                }
            }
            
        }
        #endregion

        #region Properties

        /// <summary>
        /// The rotors(i.e) moving parts of the machine. Current traverses through the rotors when a key is depressed(pressed).
        /// </summary>
        public ObservableCollection<Rotor> Rotors { get; private set; }
        = new ObservableCollection<Rotor>();

        /// <summary>
        /// When true, resets the position of the rotors to match the <see cref="ClassicalAlgorithm.Key"/>
        /// each time the algorithm is <see cref="Run(string, bool)"/>. Useful for back-forth communication
        /// as the rotors, if not in the same exact location, will produce different results.
        /// No resetting is done when <see cref="Run(char)"/> is called though.
        /// </summary>
        public bool AutoReset { get; set; } = false;

        /// <summary>
        /// The intention is to allow the simulation of the double step. Currently not implemented.
        /// </summary>
        public bool DoubleStep { get; set; } = false;

        /// <summary>
        /// The entry point of the current. The Germans called it sth sth(ETW in short)
        /// </summary>
        public EnigmaWheel Stator { get; set; }

        /// <summary>
        /// Reflects the current back through the wheels. Is what causes the Enigma to be able to decrypt the message properly
        /// </summary>
        public EnigmaWheel Reflector { get; set; }

        /// <summary>
        /// The current rotation of each of the rotors
        /// </summary>
        public int[] RotorConfig
        {
            get
            {
                if (Rotors == null) 
                    throw new NullReferenceException("Machine has no rotors");

                var list = new List<int>();

                foreach(var window in Windows)
                {
                    list.Add(Alphabet.IndexOf(window));
                }

                return list.ToArray();
            }
        }

        /// <summary>
        /// Returns a <see cref="char[]"/> of the characters currently visible through the window.
        /// </summary>
        public char[] Windows
        {
            get
            {
                if (Rotors == null)
                    throw new NullReferenceException("Machine has no rotors");
                var list = new List<char>();

                foreach (var rotor in Rotors)
                {
                    list.Add(rotor.Window);
                }

                return list.ToArray();
            }
        }

        #endregion

        #region Private Methods

        private void OnRotorHitNotch(Rotor rotor, bool forward)
        {
            var index=Rotors.IndexOf(rotor);

            if (index == -1) throw new NullReferenceException("The rotor that hit the notch was not found");

            if (index == Rotors.Count - 1) return;//the last rotor, no need to rotate

            index++; //Rotate the next rotor;
            Rotors[index].Rotate(forward);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Relaligns the rotors to a key, if one is avaiable, 
        /// otherwise to the index of the <see cref="ClassicalAlgorithm.Alphabet"/>
        /// </summary>
        public void ResetRotors()
        {
            int i = 0;
            foreach (var rotor in Rotors)
            {
                var window = string.IsNullOrEmpty(Key) ? Alphabet[0] : Key[i];
                rotor.Reset(window);
                i++;
            }
        }

        #endregion

        #region Logic Methods

        ///<inheritdoc/>
        public override string Run(string input, bool forward)
        {
            var output = new StringBuilder();

            foreach(var c in input) 
                output.Append(Run(c));

            if (AutoReset) ResetRotors();

            return output.ToString();
        }

        /// <summary>
        /// A special run for the Enigma. Simulates a key press and returns a result.
        /// </summary>
        /// <param name="input">The character that has been depressed</param>
        /// <returns>The output, i.e the character that will be lit in the lamp</returns>
        public virtual char Run(char input)
        {
            //Rotate the FastRotor
            Rotors[0].Rotate();

            //Get the index of the letter in the alphabet:
            int index = Alphabet.IndexOf(input);

            //Pass the current to the Stator:
            index = Stator.GetPath(index, true);

            //Pass the current through successive rotors:
            for(int i = 0; i < Rotors.Count; i++)
            {
                index = Rotors[i].GetPath(index, true);
            }
            
            //Pass the current the Reflector
            index = Reflector.GetPath(index, true);
            
            //Pass through the rotors in reverse:
            for(int i = Rotors.Count-1; i > -1; i--)
            {
                var rotor = Rotors[i];
                index = rotor.GetPath(index, false);
            }

            //Pass the current through the Stator:
            index = Stator.GetPath(index, false);

            return Alphabet[index];
            
        }
        #endregion
    }
}