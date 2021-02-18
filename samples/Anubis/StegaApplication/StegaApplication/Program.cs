using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Anubis.Imaging;

namespace StegaApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Would you like to insert data or retrieve data? You can type either: \nINSERT \nEXTRACT");
            string operation=Console.ReadLine();

            if (operation.ToUpper() == "INSERT")
            {
                await Inject();
            }
            else
            {
                await Eject();
            }
        }

        static async Task Inject()
        {
            Console.WriteLine("Enter the path to an image: ");
            string inputImagePath = Console.ReadLine();

            Console.WriteLine("Enter text to hide:");
            string hideText = Console.ReadLine();
            byte[] hideData = hideText.GetBytes<UTF8Encoding>();

            Console.WriteLine("Enter path to save output image: ");
            string outputImagePath = Console.ReadLine();



            using (FileStream fileInput = File.OpenRead(inputImagePath))
            using (FileStream fileOutput = File.OpenWrite(outputImagePath))
            using (MemoryStream streamData = new MemoryStream(hideData))
            {
                BitmapJector jector = new BitmapJector
                {
                    EOF = "\u0004", //Setting the End of File
                };

                await jector.InjectAsync(fileInput, fileOutput, streamData);
            }

            Console.WriteLine("Data successfully hidden, check the output image file");
        }

        static async Task Eject()
        {
            Console.WriteLine("Enter the path to image with hidden text: ");
            string inputImagePath = Console.ReadLine();

            string hiddenText = "";
            using (FileStream fileInput = File.OpenRead(inputImagePath))
            using (MemoryStream streamData = new MemoryStream())
            {
                BitmapJector jector = new BitmapJector
                {
                    EOF = "\u0004", //Setting the End of File
                };
                await jector.EjectAsync(fileInput, streamData);
                byte[] hiddenData = streamData.ToArray();
                hiddenText = hiddenData.GetString<UTF8Encoding>();
            }

            Console.WriteLine($"Hidden Data: {hiddenText}");
        }
    }
}