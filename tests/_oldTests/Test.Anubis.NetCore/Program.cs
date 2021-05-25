using System;
using System.IO;
using System.Threading.Tasks;
using Thismaker.Anubis.Media;

namespace Test.Anubis.NetCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter the path of the WAV file:");
            var inputPath = Console.ReadLine();

            //Console.WriteLine("Enter a path to Save the WAV file:");
            //var outputPath = Console.ReadLine();

            Console.WriteLine("Enter path to output data:");
            var dataPath = Console.ReadLine();

            

            using var inputStream = File.Open(inputPath, FileMode.Open);
            //using var outputStream = File.Create(outputPath);
            using var dataStream = File.Create(dataPath);

            var jector = new WaveFileJector();
            await jector.EjectAsync(inputStream, dataStream);

            Console.WriteLine("Operation Successful");

        }
    }
}
