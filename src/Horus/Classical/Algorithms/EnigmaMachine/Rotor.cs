using System;
using System.Collections.Generic;

namespace Thismaker.Horus.Classical
{
    /// <summary>
    /// A rotor provides utility methods for scrambling an enigma message.
    /// </summary>
    /// <remarks>
    /// This class simulates the behaviour of the Enigma machine's rotors as the current passes through them.
    /// </remarks>
    public class Rotor : EnigmaWheel
    {
        #region Private Fields
        private bool _raiseHitNotch = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an empty rotor.
        /// </summary>
        public Rotor() : base() { }

        ///<inheritdoc/>
        public Rotor(string indexing, string wiring) : base(indexing, wiring) { }

        ///<inheritdoc/>
        public Rotor(Alphabet indexing, Alphabet wiring) : base(indexing, wiring) { }

        /// <summary>
        /// Creates a rotor from the specified configuration
        /// </summary>
        /// <param name="indexing">The index etching/></param>
        /// <param name="wiring">The wiring etching</param>
        /// <param name="turnOver">The turover point f the wheel</param>
        public Rotor(Alphabet indexing, Alphabet wiring, char turnOver) : base(indexing, wiring)
        {
            TurnOver = new List<char>() { turnOver };
        }

        /// <summary>
        /// Creates a rotor from the specified configuration
        /// </summary>
        /// <param name="indexing">The index etching/></param>
        /// <param name="wiring">The wiring etching</param>
        /// <param name="turnOvers">A string, each <see cref="char"/> representing a turnover point</param>
        public Rotor(Alphabet indexing, Alphabet wiring, string turnOvers):base(indexing, wiring)
        {
            TurnOver = new List<char>(turnOvers);
        }

        /// <summary>
        /// Creates a rotor from the specified configuration
        /// </summary>
        /// <param name="indexing">The index etching/></param>
        /// <param name="wiring">The wiring etching</param>
        /// <param name="turnOver">The turover point f the wheel</param>
        public Rotor(string indexing, string wiring, char turnOver) : base(indexing, wiring)
        {
            TurnOver = new List<char>() { turnOver };
        }


        /// <summary>
        /// Creates a rotor from the specified configuration
        /// </summary>
        /// <param name="indexing">The index etching/></param>
        /// <param name="wiring">The wiring etching</param>
        /// <param name="turnOvers">A string, each <see cref="char"/> representing a turnover point</param>
        public Rotor(string indexing, string wiring, string turnOvers):base(indexing, wiring)
        {
            TurnOver = new List<char>(turnOvers);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Raised whenever the rotor hits a notch, i.e a turnover point becomes visible in the window.
        /// The second argument is true if the notch was hit in a counter-clockwise fasion. Otherwise false.
        /// </summary>
        public event Action<Rotor, bool> HitNotch;

        /// <summary>
        /// A list of the points that raise the <see cref="HitNotch"/>
        /// whenever they become visible through the window.
        /// </summary>
        public List<char> TurnOver { get; set; }

        //public int Rotation { get; set; }

        /// <summary>
        /// The character currently visible through the window
        /// </summary>
        public char Window { get { return Indexing[0]; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Rotates the wheel, forward or backwards.
        /// </summary>
        /// <param name="forward">If true, rotates the wheel counterclockwise</param>
        public void Rotate(bool forward=true)
        {
            int offset = forward ? 1 : -1;
            Indexing.Rotate(offset);
            Wiring.Rotate(offset);

            if (_raiseHitNotch)
            {
                HitNotch?.Invoke(this, forward);
                _raiseHitNotch = false;
            }
            else if (IsTurnOver())
            {
                _raiseHitNotch = true;
            }
        }

        /// <summary>
        /// Resets the rotor so that the given character is visible in the window.
        /// Does not raise the <see cref="HitNotch"/> event.
        /// </summary>
        /// <param name="window">The charcter desired to be in the window</param>
        public void Reset(char window)
        {
            var offset = Indexing.IndexOf(window);

            Indexing.Rotate(offset);
            Wiring.Rotate(offset);
        }

        /// <summary>
        /// Checks if the rotor is in a turn over position, i.e a turnover character
        /// is currently visible through the window
        /// </summary>
        /// <returns>True if the rotor is in it's turnover position</returns>
        public bool IsTurnOver()
        {
            return TurnOver.Contains(Indexing[0]);
        }
        #endregion
        
    }
}
