using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Enigma.Classical
{
    public static class StandardEnigmaMachines
    {
        public static EnigmaMachine A133
        {
            get =>
                new EnigmaMachine
                {
                    Alphabet = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ"),
                    Reflector = StandardRotors.A133_UKW,
                    Stator = StandardRotors.A133_ETW,
                    Rotors = new List<Rotor> 
                    {
                        StandardRotors.A133_I,
                        StandardRotors.A133_II,
                        StandardRotors.A133_III
                    }
                };
        }
    }

    public static class StandardRotors
    {
        public static EMWheel A133_ETW { get => new EMWheel("ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ"); }
        public static EMWheel A133_UKW { get => new EMWheel("LDGBÄNCPSKJAVFZHXUIÅRMQÖOTEY"); }
        public static Rotor A133_I { get => new Rotor("PSBGÖXQJDHOÄUCFRTEZVÅINLYMKA", 'Ä'); }
        public static Rotor A133_II { get => new Rotor("CHNSYÖADMOTRZXBÄIGÅEKQUPFLVJ", 'Ä'); }
        public static Rotor A133_III { get => new Rotor("ÅVQIAÄXRJBÖZSPCFYUNTHDOMEKGL", 'Ä'); }
       
    }
}
