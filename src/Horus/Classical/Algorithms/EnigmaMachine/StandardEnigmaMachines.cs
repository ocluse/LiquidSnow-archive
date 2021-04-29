namespace Thismaker.Horus.Classical
{
    /// <summary>
    /// A collection of well know Enigma Machines at their default configurations.
    /// </summary>
    public static class StandardEnigmaMachines
    {
        /// <summary>
        /// A133 is a special variant of EnigmaB, that was delivered to the Swedish SGS on 6 April 1925.
        /// It uses 28 letters, with letters Å, Ä and Ö common in the Swedish language. It lacks letter W though.
        /// </summary>
        public static EnigmaMachine A133
        {
            get=>
                new EnigmaMachine(new Rotor[]
                    {
                        StandardWheels.A133_I, 
                        StandardWheels.A133_II, 
                        StandardWheels.A133_III
                    })
                {
                    Alphabet = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ"),
                    Stator = StandardWheels.A133_ETW,
                    Reflector = StandardWheels.A133_UKW,
                };
        }

        /// <summary>
        /// Enigma D, or Commercial Enigma A26,  was introduced in 1926 and served as the basis for most
        /// of the later machines.
        /// </summary>
        public static EnigmaMachine A26
        {
            get =>
                new EnigmaMachine(new Rotor[]
                {
                        StandardWheels.A26_I,
                        StandardWheels.A26_II,
                        StandardWheels.A26_III
                })
                {
                    Alphabet = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"),
                    Stator = StandardWheels.A26_ETW,
                    Reflector = StandardWheels.A26_UKW
                };
        }

        /// <summary>
        /// The Enigma I was the main machine used by the German Army and Air Force, <i>Luftwaffe</i>. UKW_B was the 
        /// standard reflector.
        /// </summary>
        public static EnigmaMachine Enigma_I
        {
            get =>
                new EnigmaMachine(new Rotor[]
                {
                    StandardWheels.E1_I,
                    StandardWheels.E1_II,
                    StandardWheels.E1_III
                })
                {
                    Alphabet = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"),
                    Stator = StandardWheels.E1_ETW,
                    Reflector = StandardWheels.E1_UKW_B
                };
        }

        /// <summary>
        /// Immediately after the War in 1945, some captured Enigma-I machines were used by the former 
        /// Norwegian Police Security Service: <i>Overvaakingspolitiet.</i> They modified the wiring
        /// and the UKW. The ETW and position of turnover notches were left unaltered.
        /// </summary>
        public static EnigmaMachine Norenigma
        {
            get =>
                new EnigmaMachine(new Rotor[]
                {
                        StandardWheels.NE_I,
                        StandardWheels.NE_II,
                        StandardWheels.NE_III
                })
                {
                    Alphabet = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"),
                    Stator = StandardWheels.NE_ETW,
                    Reflector = StandardWheels.NE_UKW
                };
        }

        /// <summary>
        /// In the late 1980s, a strange Enigma machine was discovered in the house of a former intelligence officer,
        /// who used to work for a special unit. It was a strandard Enigma I with the UKW changed.
        /// The wheels were marked with the letter <b>S</b>, which probably means <i>Sondermaschine</i>(special machine)
        /// </summary>
        public static EnigmaMachine Sondermaschine
        {
            get =>
                new EnigmaMachine(new Rotor[]{
                        StandardWheels.SE_I,
                        StandardWheels.SE_II,
                        StandardWheels.SE_III
                    })
                {
                    Alphabet = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"),
                    Stator = StandardWheels.SE_ETW,
                    Reflector = StandardWheels.SE_UKW
                };
        }

        /// <summary>
        /// The M1, M2 and M3 Enigma machines were used by the German Navy(<i>Kriegsmarine</i>). They
        /// are basically compatible with Enigma I.
        /// </summary>
        public static EnigmaMachine Enigma_M3
        {
            get =>
                new EnigmaMachine(new Rotor[]
                {
                        StandardWheels.M3_I,
                        StandardWheels.M3_II,
                        StandardWheels.M3_III
                })
                {
                    Alphabet = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"),
                    Stator = StandardWheels.M3_ETW,
                    Reflector = StandardWheels.M3_UKW_B
                };
        }

        /// <summary>
        /// This was a further development of the M3 and was used exclusively by the U-boat division of the 
        /// Kriegsmarine. It actually featured four rotors, unlike all the other Enigma Machines. Its extra wheels had 
        /// two notches, instead of the standard one.
        /// </summary>
        public static EnigmaMachine Enigma_M4
        {
            get =>
                new EnigmaMachine(new Rotor[]
                {
                        StandardWheels.M4_I,
                        StandardWheels.M4_II,
                        StandardWheels.M4_III,
                        StandardWheels.M4_IV
                })
                {
                    Alphabet = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ"),
                    Stator = StandardWheels.M4_ETW,
                    Reflector = StandardWheels.M4_UKW_B
                };
        }

        public static EnigmaMachine RandomASCII
        {
            get => new EnigmaMachineBuilder().Build();
        }
    }

    /// <summary>
    /// A collection of well-known Enigma rotors, stators(ETW), and reflectors(UKW)
    /// at their default wiring.
    /// </summary>
    public static class StandardWheels
    {
        #region Enigma B - A133
        public static EnigmaWheel A133_ETW 
        { 
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ");
        }
        public static EnigmaWheel A133_UKW 
        { 
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ","LDGBÄNCPSKJAVFZHXUIÅRMQÖOTEY"); 
        }
        public static Rotor A133_I 
        { 
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "PSBGÖXQJDHOÄUCFRTEZVÅINLYMKA", 'Ä'); 
        }
        public static Rotor A133_II 
        { 
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "CHNSYÖADMOTRZXBÄIGÅEKQUPFLVJ", 'Ä'); 
        }
        public static Rotor A133_III 
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "ÅVQIAÄXRJBÖZSPCFYUNTHDOMEKGL", 'Ä'); 
        }
        #endregion

        #region Enigma D - A26

        public static EnigmaWheel A26_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "QWERTZUIOASDFGHJKPYXCVBNML");
        }
        public static EnigmaWheel A26_UKW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "IMETCGFRAYSQBZXWLHKDVUPOJN");
        }

        public static Rotor A26_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "LPGSZMHAEOQKVXRFYBUTNICJDW", 'Y');
        }

        public static Rotor A26_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "SLVGBTFXJQOHEWIRZYAMKPCNDU", 'E');
        }

        public static Rotor A26_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "CJGDPSHKTURAWZXFMYNQOBVLIE", 'N');
        }

        #endregion

        #region Enigma I

        public static EnigmaWheel E1_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        public static EnigmaWheel E1_UKW_A
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EJMZALYXVBWFCRQUONTSPIKHGD");
        }

        public static EnigmaWheel E1_UKW_B
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "YRUHQSLDPXNGOKMIEBFZCWVJAT");
        }

        public static EnigmaWheel E1_UKW_C
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FVPJIAOYEDRZXWGCTKUQSBNMHL");
        }

        public static Rotor E1_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
        }

        public static Rotor E1_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
        }

        public static Rotor E1_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
        }

        public static Rotor E1_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J');
        }

        public static Rotor E1_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VZBRGITYUPSDNHLXAWMJQOFECK", 'Z');
        }

        #endregion

        #region Norenigma
        //Norway Enigma
        public static EnigmaWheel NE_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        public static EnigmaWheel NE_UKW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "MOWJYPUXNDSRAIBFVLKZGQCHET");
        }

        public static Rotor NE_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "WTOKASUYVRBXJHQCPZEFMDINLG", 'Q');
        }

        public static Rotor NE_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "GJLPUBSWEMCTQVHXAOFZDRKYNI", 'E');
        }

        public static Rotor NE_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "JWFMHNBPUSDYTIXVZGRQLAOEKC", 'V');
        }

        public static Rotor NE_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FGZJMVXEPBWSHQTLIUDYKCNRAO", 'J');
        }

        public static Rotor NE_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "HEJXQOTZBVFDASCILWPGYNMURK", 'Z');
        }

        #endregion

        #region Sondermaschine
        //Sonder Enigma
        public static EnigmaWheel SE_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        public static EnigmaWheel SE_UKW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "CIAGSNDRBYTPZFULVHEKOQXWJM");
        }

        public static Rotor SE_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VEOSIRZUJDQCKGWYPNXAFLTHMB", 'Q');
        }

        public static Rotor SE_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "UEMOATQLSHPKCYFWJZBGVXINDR", 'E');
        }

        public static Rotor SE_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "TZHXMBSIPNURJFDKEQVCWGLAOY", 'V');
        }
        #endregion

        #region Enigma M3

        public static EnigmaWheel M3_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }

        public static EnigmaWheel M3_UKW_B
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "YRUHQSLDPXNGOKMIEBFZCWVJAT");
        }

        public static EnigmaWheel M3_UKW_C
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FVPJIAOYEDRZXWGCTKUQSBNMHL");
        }

        public static Rotor M3_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
        }

        public static Rotor M3_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
        }

        public static Rotor M3_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
        }

        public static Rotor M3_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J');
        }

        public static Rotor M3_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VZBRGITYUPSDNHLXAWMJQOFECK", 'Z');
        }

        public static Rotor M3_VI
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "JPGVOUMFYQBENHZRDKASXLICTW", "ZM");
        }

        public static Rotor M3_VII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "NZJHGRCXMYSWBOUFAIVLPEKQDT", "ZM");
        }

        public static Rotor M3_VIII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FKQHTLXOCBJSPDZRAMEWNIUYGV", "ZM");
        }
        #endregion

        #region Enigma M4

        public static EnigmaWheel M4_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }

        public static EnigmaWheel M4_UKW_Beta
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "LEYJVCNIXWPBQMDRTAKZGFUHOS");
        }

        public static EnigmaWheel M4_UKW_Gamma
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FSOKANUERHMBTIYCWLQPZXVGJD");
        }

        public static EnigmaWheel M4_UKW_B
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ENKQAUYWJICOPBLMDXZVFTHRGS");
        }

        public static EnigmaWheel M4_UKW_C
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "RDOBJNTKVEHMLFCWZAXGYIPSUQ");
        }

        public static Rotor M4_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
        }

        public static Rotor M4_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
        }

        public static Rotor M4_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
        }

        public static Rotor M4_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J');
        }

        public static Rotor M4_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VZBRGITYUPSDNHLXAWMJQOFECK", 'Z');
        }

        public static Rotor M4_VI
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "JPGVOUMFYQBENHZRDKASXLICTW", "ZM");
        }

        public static Rotor M4_VII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "NZJHGRCXMYSWBOUFAIVLPEKQDT", "ZM");
        }

        public static Rotor M4_VIII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FKQHTLXOCBJSPDZRAMEWNIUYGV", "ZM");
        }

        #endregion
    }
}
