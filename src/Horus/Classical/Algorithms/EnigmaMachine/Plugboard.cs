using System;
using System.Collections.Generic;

namespace Thismaker.Horus.Classical
{
    /// <summary>
    /// Provides utlilty methods for further scrambling the output of the Enigma Machine.
    /// </summary>
    public class Plugboard
    {
        private readonly Dictionary<char, char> _socketPairs;

        /// <summary>
        /// Creates a new instane with an empty socket pair.
        /// </summary>
        public Plugboard()
        {
            _socketPairs = new Dictionary<char, char>();
        }

        /// <summary>
        /// Returns the appropriate character depending on the input.
        /// If no pairs are available, returns the character itself
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public char Simulate(char input)
        {
            if (_socketPairs.ContainsKey(input)) return _socketPairs[input];
            else return input;
        }

        /// <summary>
        /// Adds a socket pair to the plugboard, which means that the plugboard
        /// interchanges the pairs when <see cref="Simulate(char)"/> is called
        /// </summary>
        /// <param name="x">A character in the pair</param>
        /// <param name="y">Another character in the pair</param>
        /// <returns></returns>
        public Plugboard AddSocketPair(char x, char y)
        {
            //Check if it's already paired
            if (_socketPairs.ContainsKey(x))
            {
                if (_socketPairs[x] == y) return this;
                else
                {
                    throw new InvalidOperationException("A character in the pair is already paired to a different character");
                }
            }

            //pair:
            _socketPairs.Add(x, y);
            _socketPairs.Add(y, x);

            return this;
        }

        /// <summary>
        /// Removes a plugboard pair.
        /// </summary>
        /// <param name="pairItem">Any character in the pair</param>
        /// <returns></returns>
        public Plugboard RemoveSocketPair(char pairItem)
        {
            if (_socketPairs.ContainsKey(pairItem))
            {
                var other = _socketPairs[pairItem];
                _socketPairs.Remove(other);
                _socketPairs.Remove(pairItem);
            }
            return this;

        }
    }
}
