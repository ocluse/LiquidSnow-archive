using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Enigma;
using TEnigma = Thismaker.Enigma.Enigma;

namespace Thismaker.Aretha
{
    class EnigmaSoul:ISoul
    {
        public Soul Soul { get { return Soul.Enigma; } }

        public void Speak(string text)
        {
            Aretha.Speak(text, Soul.Enigma);
        }

        public string Listen(string question=null, bool isYN = false)
        {
            return Aretha.Listen(isYN, Soul.Enigma);
        }

        public string GetPath(string text, bool input, bool confirm = true)
        {
            return Aretha.GetPath(text, input, confirm, Soul.Enigma);
        }
        public async void Summon(string[] args=null)
        {
            //Aretha.Speak(message);

            await WaitForCommand(args);

            Aretha.Speak("Enigma dismissed. Do you want to summon Enigma again? (Y/N)");

            var response = Aretha.Listen(true);

            if (response == null) return;

            if (response == "y")
            {
                Summon();
            }
        }

        public async Task WaitForCommand(string[] args)
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

            try
            {
                switch (cmdlets[0])
                {
                    case "playfair":
                        await Playfair(cmdlets[1]);
                        return;
                    case "aes":
                        await Aes(cmdlets[1]);
                        return;
                    default:
                        throw new InvalidOperationException("Unknown operation");
                }
            }
            catch(Exception ex)
            {
                Aretha.SoulFailed(Soul.Enigma, ex);
                return;
            }
        }

        private Task Aes(string cmd)
        {
            try
            {
                var input = GetPath("Provide the path to the input file: ", true);
                if (input == null) return Task.CompletedTask;

                var output = GetPath("Provide a path to where the output will be written: ", false);
                if (output == null) return Task.CompletedTask;

                Speak("Enter the key:");
                var key = Listen();
                if (key == null) return Task.CompletedTask;

                Speak("Operation Started");

                var keyBytes = key.GetBytes<UTF8Encoding>();
                var inputBytes = File.ReadAllBytes(input);

                var outputBytes = cmd == "encrypt" ?
                    TEnigma.AESEncrypt(inputBytes, keyBytes) :
                    TEnigma.AESDecrypt(inputBytes, keyBytes);

                File.WriteAllBytes(output, outputBytes);

                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            
        }

        private Task Playfair(string cmd)
        {
            try
            {
                Speak("Enter the input:");

                var input = Listen();
                if (input == null) return Task.CompletedTask;

                Speak("Enter the key:");

                var key = Listen();
                if (key == null) return Task.CompletedTask;

                Speak("Provide the character to omit:");
                var omit = Listen();
                if (omit == null) return Task.CompletedTask;

                var output = cmd == "encrypt" ?
                    Classical.PlayfairCipher(input, key, omit[0]) :
                    Classical.PlayfairDecipher(input, key, omit[0]);



                Speak($"OUTPUT: {output}");

                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            
        }
    }
}
