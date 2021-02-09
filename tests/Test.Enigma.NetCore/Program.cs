using System;
using Thismaker.Enigma.Classical;

namespace Test.Enigma.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a Key:");
            var key = Console.ReadLine();
            Console.WriteLine("Enter a PlainText");

            var plain = Console.ReadLine();

            var ceasar = StandardEnigmaMachines.A133;
            ceasar.Key = key;
            ceasar.AutoReset = true;
            ceasar.ResetRotors();

            var vig = new Vigenere()
            {
                Key = key,
                Alphabet = Alphabets.ASCII
            };

            var outVig = vig.Encrypt(plain);
            var outCs = ceasar.Encrypt(plain);

            Console.WriteLine($"Ceasar Cipher: {outCs}\nVigenere Cipher: {outVig}");

            Console.WriteLine($"Ceasar Decipher: {ceasar.Decrypt(outCs)}\nVigenere Decipher: {vig.Decrypt(outVig)}");
        }
    }
}
