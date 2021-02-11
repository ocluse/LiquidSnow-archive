using System;
using Thismaker.Enigma.Classical;

namespace Test.Enigma.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var enigma = StandardEnigmaMachines.RandomASCII;

            Console.WriteLine("Enter a Key:");
            var key = Console.ReadLine();
            Console.WriteLine("Enter a PlainText");

            var plain = Console.ReadLine();

            //var ceasar = new EnigmaMachine
            //{
            //    Key = key,
            //    Alphabet = new Alphabet("ABCD"),
            //    DoubleStep = false,
            //    Reflector = new EMWheel("CDAB"),
            //    Stator = new EMWheel("ABCD"),
            //    Rotors = new System.Collections.Generic.List<Rotor>
            //    {
            //        new Rotor("CBDA", 'D'),
            //        new Rotor("DACB", 'D'),
            //        new Rotor("CABD", 'D')
            //    },
            //    AutoReset=true
            //};

            

            enigma.Key = key;
            enigma.AutoReset = true;
            enigma.ResetRotors();

            
            var output = enigma.Encrypt(plain);

            Console.WriteLine($"Cipher: {output}");

            Console.WriteLine($"Decipher: {enigma.Decrypt(output)}");

            Main(args);
        }
    }
}
