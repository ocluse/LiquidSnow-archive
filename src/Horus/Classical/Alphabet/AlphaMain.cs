using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Horus.Classical
{
    public partial class Alphabet 
    {
        private readonly List<char> items;

        public Alphabet()
        {
            items = new List<char>();
        }

        public Alphabet(string characters)
        {
            items = new List<char>();

            AddAll(characters);
        }

        public Alphabet(IEnumerable<char> characters)
        {
            items = new List<char>();
            AddAll(characters);

        }

        public Dimensions Dimensions { get; set; }

        public bool IsPerfectSquare => ((double)items.Count).IsPerfectSquare();

        #region Common

        public void AddAll(IEnumerable<char> characters)
        {
            foreach (var c in characters)
            {
                Add(c);
            }
        }

        public void AddAll(string characters)
        {
            AddAll(characters.ToCharArray());
            
        }

        public void RemoveAll(string characters)
        {
            foreach(var c in characters)
            {
                Remove(c);
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach(var c in this)
            {
                builder.Append(c);
            }

            return builder.ToString();
        }
        #endregion

        #region LogicMethods

        public void Rotate(int offset)
        {
            items.Rotate(offset);
        }

        public void Shuffle()
        {
            items.Shuffle();
        }

        public void AutoDimensions()
        {
            //Prioritize Sqrt:
            if (IsPerfectSquare)
            {
                var root = (int)Math.Sqrt(Count);
                Dimensions = new Dimensions(root, root);
                return;
            }

            var factor = (int)MaxFactor((ulong)Count);

            Dimensions = new Dimensions(factor, Count / factor);
        }

        public char this[Dimensions dimension]
        {
            get
            {
                return this[dimension.X, dimension.Y];
            }
            set
            {
                this[dimension.X, dimension.Y] = value;
            }
        }

        public char this[int x, int y]
        {
            get
            {
                var index = GetDimensionalIndex(x, y);
                return this[index];
            }
            set
            {
                var index = GetDimensionalIndex(x, y);
                this[index] = value;
            }
        }

        public Dimensions DimensionsOf(char item)
        {
            var index = IndexOf(item);

            if (index == -1)
            {
                throw new NullReferenceException("Not found in alphabet");
            }
            var x = index % Dimensions.X;
            var y = index / Dimensions.X;

            return new Dimensions(x, y);
        }

        static private ulong MaxFactor(ulong n)
        {
            unchecked
            {
                while (n > 3 && 0 == (n & 1)) n >>= 1;

                uint k = 3;
                ulong k2 = 9;
                ulong delta = 16;
                while (k2 <= n)
                {
                    if (n % k == 0)
                    {
                        n /= k;
                    }
                    else
                    {
                        k += 2;
                        if (k2 + delta < delta) return n;
                        k2 += delta;
                        delta += 8;
                    }
                }
            }

            return n;
        }

        public int GetDimensionalIndex(int x, int y)
        {
            if (x > Dimensions.X) throw new InvalidOperationException("x greater than the dimension");
            if (y >= Dimensions.Y) throw new InvalidOperationException("y greater than the dimension");

            return (Dimensions.X * y) + x;
        }

        public char WrapChar(char item, int steps)
        {
            var index = IndexOf(item);

            if (index == -1) throw new InvalidOperationException("Character not found in alphabet");

            index += steps;
	    
	    while ( index >= Count ) index -= Count;

	    while ( index < 0 ) index += Count;

            return this[index];
        }

        public void Move(char item, int newIndex)
        {
            items.Move(item, newIndex);
        }

        #endregion

    }

    public struct Dimensions : IEquatable<Dimensions> 
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public Dimensions(int x, int y) { X = x; Y = y; }

        public int Size { get { return X * Y; } }

        public void Limit(Dimensions dimensions)
        {
            Limit(dimensions.X, dimensions.Y);
        }

        public void Limit(int max_x, int max_y)
        {
            X %= max_x;
            Y %= max_y;
        }

        public bool Equals(Dimensions other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Dimensions)) return false;
            var other = (Dimensions)obj;

            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (Dimensions d1, Dimensions d2)
        {
            return d1.Equals(d2);
        }

        public static bool operator !=(Dimensions d1, Dimensions d2)
        {
            return !d1.Equals(d2);
        }

        public override string ToString()
        {
            return $"{X}x{Y}";
        }
    }
}
