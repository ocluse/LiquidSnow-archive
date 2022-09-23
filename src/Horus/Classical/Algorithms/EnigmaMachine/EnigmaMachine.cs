using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Thismaker.Horus.Classical
{
    ///<summary>
    ///Provides functionality for performing cryptographic operations by simulating the Enigma Machine
    ///</summary>
    /// <remarks>
    /// The Enigma machine was used by the German army during World War 2 for top secret communication.
    /// While this class does it's best to simulate the behaviour of the physical device,
    /// there may still be a few places it falls short. Configuration is necessary to obtain desirable behaviour.
    /// </remarks>
    public partial class EnigmaMachine : ClassicalAlgorithm
    {

        #region Constructors
        /// <summary>
        /// Creates a new instance of an Enigma Machine
        /// </summary>
        public EnigmaMachine()
        {
            Rotors.CollectionChanged += OnRotorsChanged;
        }

        /// <summary>
        /// Creates a new instance of an Enigma Machine with the provided rotors
        /// </summary>
        /// <param name="rotors">The rotors to be used by the Enigma Machine</param>
        public EnigmaMachine(IEnumerable<Rotor> rotors)
        {
            Rotors.CollectionChanged += OnRotorsChanged;

            Rotors.AddRange(rotors);
        }

        private void OnRotorsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.NewItems)
                {
                    var rotor = (Rotor)item;
                    rotor.HitNotch += OnRotorHitNotch;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove
                || e.Action == NotifyCollectionChangedAction.Replace
                || e.Action == NotifyCollectionChangedAction.Reset)
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
        /// Gets or sets rotors of the Enigma Machine. 
        /// </summary>
        /// <remarks>
        /// The rotors are the moving parts of the machine. Current traverses through the rotors when a key is pressed and goes through the wiring, 
        /// changing contact points from one rotor to the next. This action scarmbles the letters
        /// </remarks>
        public ObservableCollection<Rotor> Rotors { get; private set; }
        = new ObservableCollection<Rotor>();

        /// <summary>
        /// Gets or sets a value indicating whether the rotors should be reset to the key position after each run.
        /// </summary>
        public bool AutoReset { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating if double stepping should be simulated.
        /// </summary>
        public bool DoubleStep { get; set; } = false;

        /// <summary>
        /// Gets or sets the ETW of the machine.
        /// </summary>
        /// <remarks>
        /// This is the first 'wheel' that the electric current flows into before heading to the actual rotors.
        /// It is fixed and does not rotate.
        /// </remarks>
        public EnigmaWheel Stator { get; set; }

        /// <summary>
        /// Gets or sets the reflector of the machine
        /// </summary>
        /// <remarks>
        /// Reflects the electric current back through the wheels. This action is what enables the Enigma encryption to be reversable.
        /// </remarks>
        public EnigmaWheel Reflector { get; set; }

        /// <summary>
        /// Gets or sets the machine's plugboard.
        /// </summary>
        /// <remarks>
        /// The plugboard switch allows for further scrambling by substituting character pairs.
        /// </remarks>
        public Plugboard Plugboard { get; set; }

        /// <summary>
        /// Gets the current rotation of each of the rotors.
        /// </summary>
        /// <remarks>
        /// Returns an array of integers, with each representing the index of rotation of the currently visible character through the window.
        /// </remarks>
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
        /// Returns a <see cref="char"/> array of the characters currently visible through the window.
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

            //Perform any necessary double stepping
            if (DoubleStep && index+1>=Rotors.Count)
            {
                if (Rotors[index + 1].IsTurnOver())
                {
                    Rotors[index + 1].Rotate();
                }
            }

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

            var result= Alphabet[index];

            //perform plugboard simulation:
            if (Plugboard != null)
            {
                result = Plugboard.Simulate(result);
            }
            return result;
            
        }
        #endregion
    }
}