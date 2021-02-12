using System;
using System.IO;
using Thismaker.Enigma.Classical;

namespace Test.Enigma.NetCore
{
    class Program
    {
        static EnigmaMachine enigma;
        static void Main(string[] args)
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
            Console.WriteLine("The EnigmaMachine is now Activated. Any Key you pressed will be remapped. Pressing escape will allow you to exit.");
            while (true)
            {
                var input = Console.ReadKey(true);

                if (input.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    continue;
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
