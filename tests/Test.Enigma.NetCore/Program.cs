using System;
using System.IO;
using System.Text;
using Thismaker.Horus.Classical;
using Thismaker.Horus.Symmetrics;

namespace Test.Enigma.NetCore
{
    class Program
    {
        static EnigmaMachine enigma;
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a Key: ");
            var key = Console.ReadLine().GetBytes<UTF8Encoding>();

            Console.WriteLine("Enter a plaintext");
            var input = Console.ReadLine().GetBytes<UTF8Encoding>();

            var crypter = PredefinedSymmetric.AesFixed;

            using var msInput = new MemoryStream(input);
            using var msOutput = new MemoryStream();
            crypter.Run(msInput, msOutput, key, true);

            var output = msOutput.ToArray();

            var input2 = new MemoryStream(output);
            var output2 = new MemoryStream();

            crypter.Run(input2, output2, key, false);

            Console.WriteLine($"Output: {output2.ToArray().GetString<UTF8Encoding>()}");
        }

        static void Mainx(string[] args)
        {
            Console.WriteLine("Do you wish to create a new machine? [Y/N] ");
            var input = Console.ReadLine();

            if (input.ToLower() == "y")
            {
                enigma = StandardEnigmaMachines.RandomASCII;

                Console.WriteLine("Do you wish to save this machine [Y/N]");
                input = Console.ReadLine();

                if (input.ToLower() == "y")
                {
                    Console.WriteLine("Provide a path where the machine will be saved:");
                    input = Console.ReadLine();

                    using var fs = new FileStream(input, FileMode.OpenOrCreate, FileAccess.Write);
                    enigma.Save(fs);
                }
            }
            else
            {
                Console.WriteLine("Provide path to machine to load:");
                input = Console.ReadLine();

                using var fs = new FileStream(input, FileMode.Open, FileAccess.Read);
                enigma = EnigmaMachine.Load(fs);
            }

            Operate();
        }

        private static void Operate()
        {
            GetKey();

            Work();

            Operate();
        }

        private static void GetKey()
        {
            Console.WriteLine("Please provide a key to use:");
            enigma.Key = Console.ReadLine();
            enigma.ResetRotors();
        }

        private static void Work()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The EnigmaMachine is now Activated. Any Key you pressed will be remapped. Pressing escape will allow you to exit.");
            Console.ResetColor();
            while (true)
            {
                var input = Console.ReadKey(true);

                if (input.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Machine Exited");
                    Console.ResetColor();
                    break;
                }
                if (input.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    continue;
                }
                var output = enigma.Run(input.KeyChar);
                Console.Write(output);
            }
        }
    }
}
