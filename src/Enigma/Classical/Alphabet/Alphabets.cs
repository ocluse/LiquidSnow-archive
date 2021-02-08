using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Enigma.Classical
{
    public static class Alphabets
    {
        public static Alphabet ASCII { get; private set; }

        public static Alphabet ASCIIPerfect { get; private set; }

        public static Alphabet EnglishCaps { get; private set; }

        public static Alphabet EnglishSmall { get; private set; }

        public static Alphabet AlphaNumeric { get; private set; }

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

        public static Alphabet CodePage437 { get; private set; }

        static Alphabets()
        {
            CreateAlphas();
            CreateASCII();
            CreateCP437();

            EnglishCaps.AutoDimensions();
            EnglishSmall.AutoDimensions();
            AlphaNumeric.AutoDimensions();
            ASCII.AutoDimensions();
            ASCIIPerfect.AutoDimensions();
            
        }

        private static void CreateASCII()
        {
            ASCII = new Alphabet();
            ASCIIPerfect = new Alphabet();
            for (int i = 0x20; i <= 0x7e; i++)
            {
                char c = Convert.ToChar(i);
                ASCII.Add(c);
                ASCIIPerfect.Add(c);
            }

            ASCIIPerfect.AddAll("âéìöú");
        }

        private static void CreateAlphas()
        {
            EnglishCaps = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            
            EnglishSmall = new Alphabet("abcdefghijklmnopqrstuvwxyz");
            
            AlphaNumeric = new Alphabet("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
            
        }


        private static void CreateCP437()
        {
            CodePage437 = new Alphabet();

            CodePagesEncodingProvider.Instance.GetEncoding(437);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Encoding cp437 = Encoding.GetEncoding(437);
            byte[] source = new byte[1];

            for (byte i = 0x20; i < 0xFF; i++)
            {
                source[0] = i;
                CodePage437.Add(cp437.GetString(source)[0]);
            }

        }

    }
}
