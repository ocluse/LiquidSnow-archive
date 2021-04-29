using System;
using System.IO;
using System.Text;
using Thismaker.Horus.Symmetrics;
using Thismaker.Horus.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Test.Enigma.NetCore
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Provide a Key:");
            var key = Console.ReadLine();

            Console.WriteLine("Provide a path to the CryptoContainer");
            var cryptoPath = Console.ReadLine();

            using var cryptoStream = new FileStream(cryptoPath, FileMode.OpenOrCreate);
            using var container = new CryptoContainer(cryptoStream, key);

            Console.WriteLine("Enter path to extract the contents of the Container");
            var outputPath = Console.ReadLine();
            await container.ExtractContainerAsync(outputPath);
        }

        static async Task Mainxx()
        {
            Console.WriteLine("Provide a Key:");
            var key = Console.ReadLine();

            Console.WriteLine("Provide a path to create the CryptoContainer");
            var cryptoPath = Console.ReadLine();

            var paths= new List<string>();

            while (true)
            {
                Console.WriteLine("Enter path to file to insert into CryptoFile:");
                var inputPath = Console.ReadLine();
                if (inputPath == "stop")
                    break;

                paths.Add(inputPath);
            }

            using var cryptoStream = new FileStream(cryptoPath, FileMode.OpenOrCreate);
            using var container = new CryptoContainer(cryptoStream, key);

            foreach(var path in paths)
            {
                using var inputStream = new FileStream(path, FileMode.Open);
                await container.AddAsync(path, inputStream, false);
            }
        }

        static async Task Paused()
        {

            Console.WriteLine("Provide a Key:");
            var key = Console.ReadLine();

            Console.WriteLine("Provide a path to create the CryptoFile");
            var cryptoPath = Console.ReadLine();

            Console.WriteLine("Enter path to file to insert into CryptoFile:");
            var inputPath = Console.ReadLine();

            using var cryptoStream = new FileStream(cryptoPath, FileMode.OpenOrCreate);
            using var inputStream = new FileStream(inputPath, FileMode.Open);

            using var ef = new CryptoFile(cryptoStream, key);
            await ef.WriteAsync(inputStream);

            Console.WriteLine("Enter path to extract the contents of the CryptoFile");
            var outputPath = Console.ReadLine();
            var outputStream = new FileStream(outputPath, FileMode.CreateNew);

            await ef.ReadAsync(outputStream);

            Console.WriteLine("Creating a copy to test the thing:");

            inputStream.Position = 0;
            using var fsTest = File.Create("test.crypto");
            await PredefinedSymmetric.AesFixed.EncryptAsync(inputStream, fsTest, key.GetBytes<UTF8Encoding>());
            ef.Dispose();
        }
    }
}
