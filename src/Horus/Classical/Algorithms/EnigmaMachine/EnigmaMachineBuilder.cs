using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Horus.Classical
{
    /// <summary>
    /// A builder for <see cref="EnigmaMachine"/>
    /// Provide parameters for easy building.
    /// If no parameters are provided, an ASCII based machine will be built.
    /// </summary>
    public class EnigmaMachineBuilder
    {
        private int _rotorCount = 3, _notchCount=1;
        private Alphabet _alphabet = Alphabets.ASCII;
        private IEnumerable<Rotor> _rotors;
        private bool  _autoReset=false;
        private EnigmaWheel _stator;
        private EnigmaWheel _reflector;
        
        /// <summary>
        /// If provided, the rotors will be created with the specified notches.
        /// The default is 1.
        /// Has no effect if <see cref="WithRotors(List{Rotor})"/> was called.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public EnigmaMachineBuilder WithNotchCount(int count)
        {
            _notchCount = count;
            return this;
        }

        /// <summary>
        /// When provided, the Enigma Machine will be created with that number of rotors.
        /// The default is 3.
        /// Has no effect if <see cref="WithRotors(List{Rotor})"/> was called.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public EnigmaMachineBuilder WithRotorCount(int count)
        {
            _rotorCount = count;
            return this;
        }

        /// <summary>
        /// If provided, the machine will autoreset. The default is <see cref="false"/>
        /// </summary>
        /// <returns></returns>
        public EnigmaMachineBuilder WithAutoReset()
        {
            _autoReset = true;
            return this;
        }

        /// <summary>
        /// When provided, the machine will use the stator and not create its own.
        /// If not provided, a default stator is created from the alphabet.
        /// </summary>
        /// <param name="stator"></param>
        /// <returns></returns>
        public EnigmaMachineBuilder WithStator(EnigmaWheel stator)
        {
            _stator = stator;
            return this;
        }

        /// <summary>
        /// When provided, the machine will use the reflector and not create it's own.
        /// If not provided, a random reflector is created from the alphabet.
        /// </summary>
        /// <param name="reflector"></param>
        /// <returns></returns>
        public EnigmaMachineBuilder WithReflector(EnigmaWheel reflector)
        {
            _reflector = reflector;
            return this;
        }

        /// <summary>
        /// WHen provided, the machine will use the rotors and not create its own.
        /// If not provied, the builder will create random rotors based on the alphabet
        /// and other provided parameters.
        /// </summary>
        /// <param name="rotors"></param>
        /// <returns></returns>
        public EnigmaMachineBuilder WithRotors(IEnumerable<Rotor> rotors)
        {
            _rotors = rotors;
            return this;
        }

        /// <summary>
        /// The alphabet to be used, in the machine and by the builder when creating the rotors and stators.
        /// When not provided, the default <see cref="Alphabets.ASCII"/> is used.
        /// </summary>
        /// <param name="alphabet"></param>
        /// <returns></returns>
        public EnigmaMachineBuilder WithAlphabet(Alphabet alphabet)
        {
            _alphabet = alphabet;
            return this;
        }

        /// <summary>
        /// Builds an <see cref="EnigmaMachine"/> with the provided parameters.
        /// </summary>
        /// <returns></returns>
        public EnigmaMachine Build()
        {

            var shuffle = new Alphabet(_alphabet.ToString());

            //the stator:
            var stator = _stator;
            if(stator==null)stator= new EnigmaWheel(_alphabet.ToString(), _alphabet.ToString());

            //the reflector
            var reflector = _reflector;
            if (reflector == null)
            {
                shuffle.Shuffle();
                reflector = new EnigmaWheel(_alphabet.ToString(), shuffle.ToString());
                reflector.Reflect();
            }

            //the rotors
            List<Rotor> rotors;
            if (_rotors == null)
            {
                rotors = new List<Rotor>();
                for (int i = 0; i < _rotorCount; i++)
                {
                    string notches = "";

                    for (int n = 0; n < _notchCount; n++)
                    {
                        char c;
                        do
                        {
                            int rnd = Horus.Random(0, _alphabet.Count);
                            c = _alphabet[rnd];
                        } while (notches.Contains(c));

                        notches += c;
                    }

                    shuffle.Shuffle();

                    var rotor = new Rotor(_alphabet.ToString(), shuffle.ToString(), notches);
                    rotors.Add(rotor);
                }
            }
            else
            {
                rotors = new List<Rotor>(_rotors);
            }

            //the actual construction
            return new EnigmaMachine(rotors)
            {
                Alphabet = _alphabet,
                AutoReset = _autoReset,
                Stator = stator,
                Reflector = reflector,
            };
            
        }
    }
}
