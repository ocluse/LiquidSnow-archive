using System.Linq;
using System.Text;

namespace Thismaker.Horus.Classical
{
    /// <summary>
    /// The playfair is a form of subsitution cipher that normally uses a grid to gauge its subsitution.
    /// It would normally require a 5x5 grid, but my implementation allows for any grid size.
    /// </summary>
    public class Playfair : ClassicalAlgorithm
    {
		/// <summary>
		/// Normally, conficts may arise where the diagraphs are the same e.g CC. In such a case, 
		/// this setting allows the user to decide whether to treat the collisions as having occurred
		/// in the same row(horizontal), or same column(vertical)
		/// </summary>
		public PrefferredOrientation PrefferredOrientation { get; set; }
		= PrefferredOrientation.Horizontal;

		///<inheritdoc/>
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
		/// <summary>
		/// Conficts will be treated as if they occurred in the same row.
		/// </summary>
		Horizontal,

		/// <summary>
		/// Conflicts will be treated as if they occurred in the same column
		/// </summary>
		Vertical
	}
}
