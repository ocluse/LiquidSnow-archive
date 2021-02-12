using System;

namespace Thismaker.Horus.Classical
{
    /// <summary>
    /// Represents one of the parts of an Enigma Machine that has scrambled input and output
    /// </summary>
    public class EnigmaWheel : ICloneable
    {
        /// <summary>
        /// In a real enigma machine, this was used to index pins of the wheel, e.g. ABCDE
        /// </summary>
        public Alphabet Indexing { get; set; }

        /// <summary>
        /// Represents the scrambled wiring of the Index. e.g if item 0 is W, then it means that the 
        /// <see cref="Indexing"/> 0 is connected to W.
        /// </summary>
        public Alphabet Wiring { get; set; }

        /// <summary>
        /// Creates a new wheel with everything null
        /// </summary>
        public EnigmaWheel() { }

        /// <summary>
        /// Creates a wheel from the specified indexing and wiring
        /// </summary>
        /// <param name="indexing">The index etchings</param>
        /// <param name="wiring">The scrambled of the indexing</param>
        public EnigmaWheel(Alphabet indexing, Alphabet wiring)
        {
            Indexing = indexing;
            Wiring = wiring;
        }

        /// <summary>
        /// Creates a wheel from the specified indexing and wiring
        /// </summary>
        /// <param name="indexing">The index etchings</param>
        /// <param name="wiring">The scrambled of the indexing</param>
        public EnigmaWheel(string indexing, string wiring)
        {
            Indexing = new Alphabet(indexing);
            Wiring = new Alphabet(wiring);
        }

        /// <summary>
        /// Can be used to quickly create a Reflector from a scrambled source.
        /// This method may produce conficts though, e.g C may end up mapping to C.
        /// </summary>
        public void Reflect()
        {
            var duplicate = new Alphabet(Wiring.ToString());
            string dealtWith = "";
            Wiring.Clear();

            for(int i = 0; i < Indexing.Count; i++)
            {

                char cw = duplicate[0];
                char ci = Indexing[i];

                if (Wiring.Contains(ci))
                {
                    cw = Indexing[Wiring.IndexOf(ci)];
                }
                else
                {
                    var index = 0;
                    do
                    {
                        cw = duplicate[index];
                        index++;
                    }
                    while(dealtWith.Contains(cw));
                    duplicate.Remove(cw);

                }

                dealtWith += ci;
                Wiring.Add(cw);
            }
        }

        /// <summary>
        /// Gets the path followed by the item at the specified pin.
        /// Traces the <see cref="Indexing"/> to the <see cref="Wiring"/>
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="forward">Whether the path is travelling from right-left(true) or in reverse, i.e left-right(false)</param>
        /// <returns>The output pin</returns>
        public virtual int GetPath(int pin, bool forward)
        {
            //get the character of the pin:
            if (forward)
            {
                return Indexing.IndexOf(Wiring[pin]);
            }
            else
            {
                return Wiring.IndexOf(Indexing[pin]);
            }
            
        }

        /// <summary>
        /// Creates a clone of the wheel. I don't know why, to be frank,
        /// I ever added this method. At the time it seemed so cool :(
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
