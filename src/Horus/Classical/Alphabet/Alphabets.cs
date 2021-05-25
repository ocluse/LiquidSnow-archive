using System;
using System.Text;

namespace Thismaker.Horus.Classical
{
    /// <summary>
    /// A collection of basic common alphabets to get you started
    /// </summary>
    public static class Alphabets
    {
        /// <summary>
        /// Features all the printable ASCII characters, from 0x20 to 0x7e
        /// </summary>
        public static Alphabet ASCII
        {
            get
            {
                var alpha = new Alphabet();
                for (int i = 0x20; i <= 0x7e; i++)
                {
                    char c = Convert.ToChar(i);
                    alpha.Add(c);
                }

                return alpha;
            }
        }

        /// <summary>
        /// Features all the printable ASCII characters, plus 5 more characters (âéìöú) 
        /// to make the letter count exactly 100, suitable for use with <see cref="Playfair"/>
        /// </summary>
        public static Alphabet ASCIIPerfect
        {
            get
            {
                var result = new Alphabet(ASCII.ToString());
                result.AddAll("âéìöú");
                result.AutoDimensions();
                return result;
            }
        }

        /// <summary>
        /// Contains 26 all caps letters of the English alphabet
        /// </summary>
        public static Alphabet EnglishCaps
        {
            get => new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }

        /// <summary>
        /// Contains 26 lower case letters of the English alphabet
        /// </summary>
        public static Alphabet EnglishSmall
        {
            get => new Alphabet("abcdefghijklmnopqrstuvwxyz");
        }

        /// <summary>
        /// Contains both lower and upper case letters, plus numbers.
        /// </summary>
        public static Alphabet AlphaNumeric
        {
            get => new Alphabet("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
        }

        /// <summary>
        /// Contains all printable characters in the Unicode whatever.
        /// Some characters may appear as ? when the font used to represent them does not contain the character.
        /// </summary>
        public static Alphabet LargeAlphabet
        {
            get
            {
                var alpha = new Alphabet();

                for (int i = char.MinValue; i <= char.MaxValue; i++)
                {
                    char c = Convert.ToChar(i);

                    if (char.IsControl(c)) continue;
                    alpha.Add(c);
                }
                alpha.AutoDimensions();
                return alpha;
            }
        }

        private static string _codePage37;

        /// <summary>
        /// A special set of characters developed by IBM. I got it somewhere online and though it was cool.
        /// It has the basic ASCII characters, plus a bunch of Greek symbols within, plus more.
        /// </summary>
        public static Alphabet CodePage437
        {
            get
            {
                if (string.IsNullOrEmpty(_codePage37)) CreateCP437();
                return new Alphabet(_codePage37);
            }
        }

        private static void CreateCP437()
        {
            var alpha = new Alphabet();
            CodePagesEncodingProvider.Instance.GetEncoding(437);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Encoding cp437 = Encoding.GetEncoding(437);
            byte[] source = new byte[1];

            for (byte i = 0x20; i < 0xFF; i++)
            {
                source[0] = i;
                alpha.Add(cp437.GetString(source)[0]);
            }

            _codePage37 = alpha.ToString();
        }
    }
}
