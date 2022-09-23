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
                    DoubleStep=true
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
                    Reflector = StandardWheels.A26_UKW,
                    DoubleStep = true
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
                    Reflector = StandardWheels.E1_UKW_B,
                    DoubleStep = true
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
                    Reflector = StandardWheels.NE_UKW,
                    DoubleStep = true
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
                    Reflector = StandardWheels.SE_UKW,
                    DoubleStep = true
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
                    Reflector = StandardWheels.M3_UKW_B,
                    DoubleStep = true
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
                    Reflector = StandardWheels.M4_UKW_B,
                    DoubleStep = true
                };
        }

        /// <summary>
        /// Creates an enigma machine with a random configuration, whose alphabet is the ASCII character set.
        /// </summary>
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
        /// <summary>
        /// The default configuration of the ETW of Enigma B - A133
        /// </summary>
        public static EnigmaWheel A133_ETW 
        { 
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ");
        }
        /// <summary>
        /// The default configuration of the UKW of Enigma B - A133
        /// </summary>
        public static EnigmaWheel A133_UKW 
        { 
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ","LDGBÄNCPSKJAVFZHXUIÅRMQÖOTEY"); 
        }
        /// <summary>
        /// The default configuration of the the first rotor of Enigma B - A133
        /// </summary>
        public static Rotor A133_I 
        { 
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "PSBGÖXQJDHOÄUCFRTEZVÅINLYMKA", 'Ä'); 
        }
        /// <summary>
        /// The default configuration of the second rotor of Enigma B - A133
        /// </summary>
        public static Rotor A133_II 
        { 
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "CHNSYÖADMOTRZXBÄIGÅEKQUPFLVJ", 'Ä'); 
        }
        /// <summary>
        /// The default configuration of the third rotor of Enigma B - A133
        /// </summary>
        public static Rotor A133_III 
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ", "ÅVQIAÄXRJBÖZSPCFYUNTHDOMEKGL", 'Ä'); 
        }
        #endregion

        #region Enigma D - A26
        /// <summary>
        /// The default configuration of the ETW of Enigma D - A26
        /// </summary>
        public static EnigmaWheel A26_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "QWERTZUIOASDFGHJKPYXCVBNML");
        }
        /// <summary>
        /// The default configuration of the UKW of Enigma D - A26
        /// </summary>
        public static EnigmaWheel A26_UKW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "IMETCGFRAYSQBZXWLHKDVUPOJN");
        }
        /// <summary>
        /// The default configuration of rotor I of Enigma D - A26
        /// </summary>
        public static Rotor A26_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "LPGSZMHAEOQKVXRFYBUTNICJDW", 'Y');
        }
        /// <summary>
        /// The default configuration of rotor II of Enigma D - A26
        /// </summary>
        public static Rotor A26_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "SLVGBTFXJQOHEWIRZYAMKPCNDU", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Enigma D - A26
        /// </summary>
        public static Rotor A26_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "CJGDPSHKTURAWZXFMYNQOBVLIE", 'N');
        }

        #endregion

        #region Enigma I
        /// <summary>
        /// The default configuration of the ETW of Enigma I
        /// </summary>
        public static EnigmaWheel E1_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        /// <summary>
        /// The default configuration of the one of the UKW of Enigma I
        /// </summary>
        public static EnigmaWheel E1_UKW_A
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EJMZALYXVBWFCRQUONTSPIKHGD");
        }
        /// <summary>
        /// The default configuration of the one of the UKW of Enigma I
        /// </summary>
        public static EnigmaWheel E1_UKW_B
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "YRUHQSLDPXNGOKMIEBFZCWVJAT");
        }
        /// <summary>
        /// The default configuration of the one of the UKW of Enigma I
        /// </summary>
        public static EnigmaWheel E1_UKW_C
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FVPJIAOYEDRZXWGCTKUQSBNMHL");
        }
        /// <summary>
        /// The default configuration of rotor I of Enigma I
        /// </summary>
        public static Rotor E1_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
        }
        /// <summary>
        /// The default configuration of rotor II of Enigma I
        /// </summary>
        public static Rotor E1_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Enigma I
        /// </summary>
        public static Rotor E1_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
        }
        /// <summary>
        /// The default configuration of rotor IV of Enigma I
        /// </summary>
        public static Rotor E1_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J');
        }
        /// <summary>
        /// The default configuration of rotor V of Enigma I
        /// </summary>
        public static Rotor E1_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VZBRGITYUPSDNHLXAWMJQOFECK", 'Z');
        }

        #endregion

        #region Norenigma
        /// <summary>
        /// The default configuration of ETW of Norenigma (Norway Enigma)
        /// </summary>
        public static EnigmaWheel NE_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        /// <summary>
        /// The default configuration of UKW of Norenigma (Norway Enigma)
        /// </summary>
        public static EnigmaWheel NE_UKW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "MOWJYPUXNDSRAIBFVLKZGQCHET");
        }
        /// <summary>
        /// The default configuration of rotor I of Norenigma (Norway Enigma)
        /// </summary>
        public static Rotor NE_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "WTOKASUYVRBXJHQCPZEFMDINLG", 'Q');
        }
        /// <summary>
        /// The default configuration of rotor II of Norenigma (Norway Enigma)
        /// </summary>
        public static Rotor NE_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "GJLPUBSWEMCTQVHXAOFZDRKYNI", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Norenigma (Norway Enigma)
        /// </summary>
        public static Rotor NE_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "JWFMHNBPUSDYTIXVZGRQLAOEKC", 'V');
        }
        /// <summary>
        /// The default configuration of rotor IV of Norenigma (Norway Enigma)
        /// </summary>
        public static Rotor NE_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FGZJMVXEPBWSHQTLIUDYKCNRAO", 'J');
        }
        /// <summary>
        /// The default configuration of rotor V of Norenigma (Norway Enigma)
        /// </summary>
        public static Rotor NE_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "HEJXQOTZBVFDASCILWPGYNMURK", 'Z');
        }

        #endregion

        #region Sondermaschine
        /// <summary>
        /// The default configuration of ETW of Sondermaschine
        /// </summary>
        public static EnigmaWheel SE_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        /// <summary>
        /// The default configuration of UKW of Sondermaschine
        /// </summary>
        public static EnigmaWheel SE_UKW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "CIAGSNDRBYTPZFULVHEKOQXWJM");
        }
        /// <summary>
        /// The default configuration of rotor I of Sondermaschine
        /// </summary>
        public static Rotor SE_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VEOSIRZUJDQCKGWYPNXAFLTHMB", 'Q');
        }
        /// <summary>
        /// The default configuration of rotor II of Sondermaschine
        /// </summary>
        public static Rotor SE_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "UEMOATQLSHPKCYFWJZBGVXINDR", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Sondermaschine
        /// </summary>
        public static Rotor SE_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "TZHXMBSIPNURJFDKEQVCWGLAOY", 'V');
        }
        #endregion

        #region Enigma M3
        /// <summary>
        /// The default configuration of ETW of Enigma M3
        /// </summary>
        public static EnigmaWheel M3_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        /// <summary>
        /// The default configuration of UKW B of Enigma M3
        /// </summary>
        public static EnigmaWheel M3_UKW_B
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "YRUHQSLDPXNGOKMIEBFZCWVJAT");
        }
        /// <summary>
        /// The default configuration of UKW C of Enigma M3
        /// </summary>
        public static EnigmaWheel M3_UKW_C
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FVPJIAOYEDRZXWGCTKUQSBNMHL");
        }
        /// <summary>
        /// The default configuration of rotor I of Enigma M3
        /// </summary>
        public static Rotor M3_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
        }
        /// <summary>
        /// The default configuration of rotor II of Enigma M3
        /// </summary>
        public static Rotor M3_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Enigma M3
        /// </summary>
        public static Rotor M3_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
        }
        /// <summary>
        /// The default configuration of rotor IV of Enigma M3
        /// </summary>
        public static Rotor M3_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J');
        }
        /// <summary>
        /// The default configuration of rotor V of Enigma M3
        /// </summary>
        public static Rotor M3_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VZBRGITYUPSDNHLXAWMJQOFECK", 'Z');
        }
        /// <summary>
        /// The default configuration of rotor VI of Enigma M3
        /// </summary>
        public static Rotor M3_VI
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "JPGVOUMFYQBENHZRDKASXLICTW", "ZM");
        }
        /// <summary>
        /// The default configuration of rotor VII of Enigma M3
        /// </summary>
        public static Rotor M3_VII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "NZJHGRCXMYSWBOUFAIVLPEKQDT", "ZM");
        }
        /// <summary>
        /// The default configuration of rotor VIII of Enigma M3
        /// </summary>
        public static Rotor M3_VIII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FKQHTLXOCBJSPDZRAMEWNIUYGV", "ZM");
        }
        #endregion

        #region Enigma M4
        /// <summary>
        /// The default configuration of ETW of Enigma M4
        /// </summary>
        public static EnigmaWheel M4_ETW
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }
        /// <summary>
        /// The default configuration of UKW Beta of Enigma M4
        /// </summary>
        public static EnigmaWheel M4_UKW_Beta
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "LEYJVCNIXWPBQMDRTAKZGFUHOS");
        }
        /// <summary>
        /// The default configuration of UKW Gamma of Enigma M4
        /// </summary>
        public static EnigmaWheel M4_UKW_Gamma
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FSOKANUERHMBTIYCWLQPZXVGJD");
        }
        /// <summary>
        /// The default configuration of UKW B of Enigma M4
        /// </summary>
        public static EnigmaWheel M4_UKW_B
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ENKQAUYWJICOPBLMDXZVFTHRGS");
        }
        /// <summary>
        /// The default configuration of UKW C of Enigma M4
        /// </summary>
        public static EnigmaWheel M4_UKW_C
        {
            get => new EnigmaWheel("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "RDOBJNTKVEHMLFCWZAXGYIPSUQ");
        }
        /// <summary>
        /// The default configuration of rotor I of Enigma M4
        /// </summary>
        public static Rotor M4_I
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
        }
        /// <summary>
        /// The default configuration of rotor II of Enigma M4
        /// </summary>
        public static Rotor M4_II
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
        }
        /// <summary>
        /// The default configuration of rotor III of Enigma M4
        /// </summary>
        public static Rotor M4_III
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
        }
        /// <summary>
        /// The default configuration of rotor IV of Enigma M4
        /// </summary>
        public static Rotor M4_IV
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J');
        }
        /// <summary>
        /// The default configuration of rotor V of Enigma M4
        /// </summary>
        public static Rotor M4_V
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "VZBRGITYUPSDNHLXAWMJQOFECK", 'Z');
        }
        /// <summary>
        /// The default configuration of rotor VI of Enigma M4
        /// </summary>
        public static Rotor M4_VI
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "JPGVOUMFYQBENHZRDKASXLICTW", "ZM");
        }
        /// <summary>
        /// The default configuration of rotor VII of Enigma M4
        /// </summary>
        public static Rotor M4_VII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "NZJHGRCXMYSWBOUFAIVLPEKQDT", "ZM");
        }
        /// <summary>
        /// The default configuration of rotor VIII of Enigma M4
        /// </summary>
        public static Rotor M4_VIII
        {
            get => new Rotor("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "FKQHTLXOCBJSPDZRAMEWNIUYGV", "ZM");
        }
        #endregion
    }
}
