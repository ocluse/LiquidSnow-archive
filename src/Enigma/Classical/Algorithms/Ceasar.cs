using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Enigma.Classical
{
    public class Ceasar : ClassicalAlgorithm
    {
        private char _key;
        public override string Key 
        {
            get => _key.ToString();
            set => _key = value[0];
        }

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
