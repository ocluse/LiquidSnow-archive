using System;
using System.Collections.Generic;
using Thismaker.Anubis;
using System.IO;
using System.Threading.Tasks;

namespace Thismaker.Aretha
{
    class AnubisSoul : ISoul
    {
       
        public Soul Soul { get { return Soul.Anubis; } }

        

        public void Speak(string text)
        {
            Aretha.Speak(text, Soul.Anubis);
        }

        public string Ask(string question=null,bool isYN = false, bool isCase=false)
        {
            return Aretha.Ask(question,isYN, Soul.Anubis, isCase);
        }

        public string GetPath(string text, bool input, bool confirm=true)
        {
            return Aretha.GetPath(text, input, confirm, Soul.Anubis);
        }

        public async void WaitForCommand(string[] args=null)
        {
            try
            {
                List<string> cmdlets;
                if (args == null || args.Length == 0)
                {
                    var cmd = Ask();
                    if (cmd == null) return;

                    cmdlets = new List<string>(cmd.Split(' '));
                }
                else
                {
                    cmdlets = new List<string>(args);
                }

                Jector jector;

                var type = GetJectorType(cmdlets[0]);

                //make the jector:
                var jectorBuilder = new JectorBuilder().WithType(type);

                var response = Ask("Do you want to ensure a successful write?", true);

                //if (response == null) return;

                if (response == "y")
                {
                    jectorBuilder.WithRequiredSuccess();
                }

                response = Ask("Do you want to use the default end of file marker?", true);
                //if (response == null) return;

                if (response == "n")
                {
                    response = Ask("What end of file marker would you like to use?", false, true);
                    //if (response == null) return;

                    jectorBuilder.WithEOF(response);
                }

                jector = jectorBuilder.Build();

                if (cmdlets[1].ToLower() == "inject")
                {
                    await Inject(jector);

                }
                else if (cmdlets[1].ToLower() == "eject")
                {
                    await Eject(jector);
                }

                Aretha.SoulSucceeded(Soul.Anubis);
            }
            catch(Exception ex)
            {
                Aretha.SoulFailed(Soul.Anubis, ex);
                return;
            }
        }

        public async Task Inject(Jector jector)
        {
            var inputPath = GetPath("Provide path to file to be written into:", true);
            var dataPath = GetPath("Provide a path to the file to be hidden:", true);
            var outputPath = GetPath("Provide a path where the file copy with hidden data will be saved:", false);

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

            try
            {
                using var source = File.OpenRead(inputPath);
                using var destination = File.OpenWrite(outputPath);
                using var data = File.OpenRead(dataPath);
                await jector.InjectAsync(source, destination, data, Aretha.WriteProgress());
                Speak($"Command Executed Successfully!");
            }
            catch(Exception ex)
            {
                Aretha.SoulFailed(Soul.Anubis, ex);
            }
        }

        public async Task Eject(Jector jector)
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

            try
            {
                using var source = File.OpenRead(inputPath);
                using var destination = File.OpenWrite(outputPath);

                await jector.EjectAsync(source, destination, Aretha.WriteProgress());

                Speak($"Command Executed Successfully!");
                
            }
            catch(Exception ex)
            {
                Aretha.SoulFailed(Soul.Anubis, ex);
            }
        }

        private static JectorType GetJectorType(string name)
        {
            if (name == "bitmap") return JectorType.Bitmap;
            if (name == "wavefile") return JectorType.WaveFile;
            throw new InvalidCastException("Unknown Jector Type");
        }
    }
}
