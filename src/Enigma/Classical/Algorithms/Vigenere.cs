using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Enigma.Classical
{
    public class Vigenere : ClassicalAlgorithm
    {
		public override string Run(string input, bool forward)
        {
			var output = new StringBuilder();

			int indexer = 0;
			foreach(var c in input)
            {
				if (indexer == Key.Length) indexer = 0;
				var steps = Alphabet.IndexOf(Key[indexer]) * (forward ? 1 : -1);

				output.Append(Alphabet.WrapChar(c, steps));
				indexer++;
            }

			return output.ToString();
        }
    }
}
