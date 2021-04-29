using System.Text;

namespace Thismaker.Horus.Classical
{
    /// <summary>
    /// The famouse Ceasar Cipher believed to have been employed by Julius Ceasar himself.
    /// Its the simplest form of subsitution cipher know.
    /// </summary>
    public class Ceasar : ClassicalAlgorithm
    {
        private char _key;

        /// <summary>
        /// Though this requires a string, only the first element of the string will be considered.
        /// The index of this first element is employed in calculating the steps.
        /// </summary>
        public override string Key 
        {
            get => _key.ToString();
            set => _key = value[0];
        }

        ///<inheritdoc/>
		public override string Run(string input, bool forward)
        {
			var steps = Alphabet.IndexOf(_key) * (forward ? 1 : -1);
			var output = new StringBuilder();

			foreach(var i in input)
            {
				 output.Append(Alphabet.WrapChar(i, steps));
            }

			return output.ToString();
        }
    }
}
