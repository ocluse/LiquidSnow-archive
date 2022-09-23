using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Horus.Classical
{
    /// <summary>
    /// Represents a collection of unique characters that are stringed together to form a message
    /// </summary>
    public partial class Alphabet 
    {
        #region Privates
        private readonly List<char> _items;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new empty alphabet
        /// </summary>
        public Alphabet()
        {
            _items = new List<char>();
        }

        /// <summary>
        /// Creates a new alphabet from the characters in the string
        /// </summary>
        /// <param name="characters"></param>
        public Alphabet(string characters)
        {
            _items = new List<char>();

            AddAll(characters);
        }

        /// <summary>
        /// Creates a new alphabet from the characters in the collection
        /// </summary>
        /// <param name="characters"></param>
        public Alphabet(IEnumerable<char> characters)
        {
            _items = new List<char>();
            AddAll(characters);

        }

        #endregion

        #region Properties
        /// <summary>
        /// The dimensions of the alphabet, typically used with Playfair cipher.
        /// This represents the rows and columns of the alphabet.
        /// </summary>
        public Dimensions Dimensions { get; set; }

        /// <summary>
        /// This means that the alphabet items are a perfect square of a number.
        /// Good for checking if square dimensions are possible
        /// </summary>
        public bool IsPerfectSquare => ((double)_items.Count).IsPerfectSquare();

        #endregion

        #region Public Methods

        /// <summary>
        /// Rotates the alphabet, wrapping the end or first items as necessary
        /// </summary>
        /// <param name="offset">The amount of rotation to apply</param>
        public void Rotate(int offset)
        {
            _items.Rotate(offset);
        }

        /// <summary>
        /// Shuffles the alphabet, reodering the items randomly.
        /// </summary>
        public void Shuffle()
        {
            _items.Shuffle();
        }

        /// <summary>
        /// Moves the character to the new index
        /// </summary>
        /// <param name="item">The character to move</param>
        /// <param name="newIndex">The new index</param>
        public void Move(char item, int newIndex)
        {
            _items.Move(item, newIndex);
        }

        /// <summary>
        /// Recreates the dimensions of the items automatically depending on the item count.
        /// For example an alphabet with 25characters gets a 5x5 dimensions while that with
        /// 12 characters gets a 4x3 dimension.
        /// </summary>
        public void AutoDimensions()
        {
            //Prioritize Sqrt:
            if (IsPerfectSquare)
            {
                var root = (int)Math.Sqrt(Count);
                Dimensions = new Dimensions(root, root);
                return;
            }

            var factor = (int)((ulong)Count).MaxFactor();

            Dimensions = new Dimensions(factor, Count / factor);
        }

        /// <summary>
        /// Returns the dimensional position of a character in the alphabet.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the logical index of the character at the specified position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetDimensionalIndex(int x, int y)
        {
            if (x > Dimensions.X) throw new InvalidOperationException("x greater than the dimension");
            if (y >= Dimensions.Y) throw new InvalidOperationException("y greater than the dimension");

            return (Dimensions.X * y) + x;
        }

        /// <summary>
        /// Starting from the index of the provided character, this method offsets
        /// that index by the number of steps and finds the character at the new index,
        /// wrapping around the alphabet as necessary. Useful for example in shifting for Ceaser Cipher
        /// </summary>
        /// <param name="item">The character to start at</param>
        /// <param name="steps">The number of steps to apply</param>
        public char WrapChar(char item, int steps)
        {
            var index = IndexOf(item);

            if (index == -1) throw new InvalidOperationException("Character not found in alphabet");

            index += steps;
            while (index >= Count) index -= Count;

            while (index < 0) index += Count;

            return this[index];
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Retrieves the charater at the point represented by the dimension
        /// </summary>
        /// <param name="dimension">The dimension representing the position of the character</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieves a character at the provided position
        /// </summary>
        /// <param name="x">The column of the character</param>
        /// <param name="y">The row of the character</param>
        /// <returns></returns>
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

        #endregion

        #region Object Overrides
        /// <summary>
        /// Gets a neatly formatted string of all the alphabet's characters
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var c in this)
            {
                builder.Append(c);
            }

            return builder.ToString();
        }
        #endregion

    }

    /// <summary>
    /// Represents 2D dimensional info with X and Y
    /// </summary>
    public struct Dimensions : IEquatable<Dimensions> 
    {
        /// <summary>
        /// The number of items in the X (Horizontal) direction.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// The number of items in the Y (Vertical) direction
        /// </summary>
        public int Y { get; set; }
        
        /// <summary>
        /// Creates a new instance using the specified dimensions
        /// </summary>
        public Dimensions(int x, int y) { X = x; Y = y; }

        /// <summary>
        /// The product of the <see cref="X"/> and <see cref="Y"/> dimensions
        /// </summary>
        public int Size { get { return X * Y; } }

        /// <summary>
        /// Truncates the dimensions to match the dimensions provided
        /// </summary>
        /// <param name="dimensions">The dimensions to be matched</param>
        public void Limit(Dimensions dimensions)
        {
            Limit(dimensions.X, dimensions.Y);
        }
        /// <summary>
        /// Truncates the dimensions to match the provided X and Y
        /// </summary>
        /// <param name="max_x">The maximum dimension in the X direction</param>
        /// <param name="max_y">The maximum dimension in the Y direction</param>
        public void Limit(int max_x, int max_y)
        {
            X %= max_x;
            Y %= max_y;
        }


        #region Object Overrides
        ///<inheritdoc/>
        public bool Equals(Dimensions other)
        {
            return X == other.X && Y == other.Y;
        }
        ///<inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Dimensions)) return false;
            var other = (Dimensions)obj;

            return X == other.X && Y == other.Y;
        }
        ///<inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// Checks if two dimensions are equal
        /// </summary>
        public static bool operator == (Dimensions d1, Dimensions d2)
        {
            return d1.Equals(d2);
        }
        /// <summary>
        /// Checks if two dimensions are not equal
        /// </summary>
        public static bool operator !=(Dimensions d1, Dimensions d2)
        {
            return !d1.Equals(d2);
        }

        /// <summary>
        /// Returns a neatly formatted string of the dimensions, e.g. 2x4
        /// </summary>
        public override string ToString()
        {
            return $"{X}x{Y}";
        }

        #endregion
    }
}
