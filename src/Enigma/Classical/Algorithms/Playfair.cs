using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thismaker.Enigma.Classical
{
    public class Playfair : ClassicalAlgorithm
    {
		public PrefferredOrientation PrefferredOrientation { get; set; }
		= PrefferredOrientation.Horizontal;

		public override string Run(string input, bool forward)
        {
            var keytable = new Alphabet(Alphabet.ToString())
            {
                Dimensions = Alphabet.Dimensions
            };

            var distinct = Key.Distinct().ToList();

			for (int i = 0; i < distinct.Count; i++)
            {
				keytable.Move(distinct[i], i);
            }

			if (input.Length % 2 != 0) input += Alphabet[Alphabet.Count-1];

			var multiplier = forward ? 1 : -1;
			var output = new StringBuilder();
			//Let the ciphering begin:
			for(int i=0; i<input.Length; i += 2)
            {
				var a = keytable.DimensionsOf(input[i]);
				var b = keytable.DimensionsOf(input[i + 1]);

				var newA = new Dimensions(b.X, a.Y);
				var newB = new Dimensions(a.X, b.Y);

                if (a.X == b.X || a.Y == b.Y)
                {
					newA = a;
					newB = b;
					//SamePoint
					if (a == b)
					{
						newA.X = PrefferredOrientation == PrefferredOrientation.Horizontal ? 
							newA.X += multiplier : newA.X;
						newA.Y = PrefferredOrientation == PrefferredOrientation.Vertical ? 
							newA.Y += multiplier : newA.Y;

						newB = new Dimensions(newA.X, newA.Y);
					}
					//SameCol
                    else if (a.X == b.X)
                    {
						newA.Y += multiplier;
						newB.Y += multiplier;
                    }
					//SameRow
					else
                    {
						newA.X += multiplier;
						newB.X += multiplier;
                    }

					newA.Limit(keytable.Dimensions);
					newB.Limit(keytable.Dimensions);
				}

				output.Append(keytable[newA]);
				output.Append(keytable[newB]);
			}

			return output.ToString();
        }
	}

	public enum PrefferredOrientation
	{
		Horizontal,
		Vertical
	}
}
