using System;
using System.Collections.Generic;
using System.Text;
using Thismaker.Anubis;
using System.IO;
using System.Threading.Tasks;

namespace Thismaker.Aretha
{
    static class AnubisSoul
    {
       
        private static Progress<float> WriteProgress()
        {
            var progress = new Progress<float>();

            int previousProgress = 0;

            progress.ProgressChanged += (o, e) =>
            {
                int percent = (int)(e * 100.0f);

                if (percent - previousProgress < 1) return;

                previousProgress = percent;

                Console.Write("\r");
                string progBar = "Execution Progress: ";
                for (int i = 0; i <= 100; i += 10)
                {
                    if (i <= percent) progBar += "|";
                    else progBar += ".";
                }

                progBar += $" {percent}%";

                

                Console.Write(progBar);

                if (percent == 100)
                {
                    Console.WriteLine();
                }
            };

            return progress;
        }

        private static void Speak(string text)
        {
            Aretha.Speak(text, Soul.Anubis);
        }

        private static string Listen(bool isYN = false)
        {
            return Aretha.Listen(isYN, Soul.Anubis);
        }

        private static string GetPath(string text, bool input, bool confirm=true)
        {
            return Aretha.GetPath(text, input, confirm, Soul.Anubis);
        }

        public static async Task Summon(string message="Anubis has been called", string[] args=null)
        {
            Aretha.Speak(message);

            await WaitForCommand(args);

            Aretha.Speak("Anubis dismissed. Do you want to summon Anubis again? (Y/N)");

            var response=Aretha.Listen(true);

            if (response == null) return;

            if (response == "y")
            {
                await Summon("Anubis has returned");
            }
        }

        public static async Task WaitForCommand(string[] args)
        {
            List<string> cmdlets;
            if (args == null || args.Length == 0)
            {
                var cmd = Listen();
                if (cmd == null) return;

                cmdlets = new List<string>(cmd.Split(' '));
            }
            else
            {
                cmdlets = new List<string>(args);
            }

            IJector jector;

            var type = GetJectorType(cmdlets[0]);

            //make the jector:
            var jectorBuilder = new JectorBuilder().WithType(type);

            Speak("Do you want to ensure a successful write? (Y/N)");

            var response = Listen(true);
            if (response == null) return;

            if (response=="y")
            {
                jectorBuilder.WithRequiredSuccess();
            }

            Speak("Do you want to use the default end of file marker? (Y/N)");
            response = Listen(true);
            if (response == null) return;

            if (response == "n")
            {
                Speak("What end of file marker would you like to use?");
                response = Listen();
                if (response == null) return;

                jectorBuilder.WithEOF(response);
            }

            jector = jectorBuilder.Build();

            if (cmdlets[1].ToLower() == "inject")
            {
                try
                {
                    await Inject(jector);
                }
                catch(Exception ex)
                {
                    Aretha.SoulFailed(Soul.Anubis, ex);
                    return;
                    
                }
                
            }
            else if (cmdlets[1].ToLower() == "eject")
            {
                try
                {
                    await Eject(jector);
                }
                catch(Exception ex)
                {
                    Aretha.SoulFailed(Soul.Anubis, ex);
                    return;
                }
            }
            
        }

        public static async Task Inject(IJector jector)
        {
            var inputPath = GetPath("Provide path to file to be written into:", true);
            if (inputPath == null) return;
            var dataPath = GetPath("Provide a path to the file to be hidden:", true);
            if (dataPath == null) return;
            var outputPath = GetPath("Provide a path where the file copy with hidden data will be saved:", false);
            if (outputPath == null) return;

            if (inputPath == outputPath)
            {
                Speak("The input file cannot be the same as the save file!");
                await Inject(jector);
                return;
            }
            else if (inputPath == dataPath)
            {
                Speak("The input file cannot be the same as the data file. What will be the point?");
                await Inject(jector);
                return;
            }else if (dataPath == outputPath)
            {
                Speak("The data file cannot be the same as the save file");
                await Inject(jector);
            }

            Speak("Executing Command. Please Wait");

            using var source = File.OpenRead(inputPath);
            using var destination = File.OpenWrite(outputPath);
            using var data = File.OpenRead(dataPath);
            await jector.InjectAsync(source, destination, data, WriteProgress());


            Speak($"Command Executed Successfully!");
        }

        public static async Task Eject(IJector jector)
        {
            var inputPath = GetPath("Provide a path to file containing hidden data:", true);

            if (inputPath == null) return;

            var outputPath = GetPath("Provide a path where the hidden file will be saved: ", false);

            if (outputPath == null) return;

            if (inputPath == outputPath)
            {
                Speak("The input file cannot be the same as the save file!");
                await Inject(jector);
                return;
            }

            Speak("Executing Command. Please Wait");

            using var source = File.OpenRead(inputPath);
            using var destination = File.OpenWrite(outputPath);

            await jector.EjectAsync(source, destination, WriteProgress());

            Speak($"Command Executed Successfully!");
        }

        private static JectorType GetJectorType(string name)
        {
            if (name == "bitmap") return JectorType.Bitmap;
            if (name == "wavefile") return JectorType.WaveFile;
            throw new InvalidCastException("Unknown Jector Type");
        }
    }
}
