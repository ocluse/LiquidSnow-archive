using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Horus;
using Thismaker.Horus.IO;
using Thismaker.Horus.Classical;

namespace Thismaker.Aretha
{
    class HorusSoul:ISoul
    {
        public Soul Soul { get { return Soul.Horus; } }

        public void Speak(string text)
        {
            Aretha.Speak(text, Soul.Horus);
        }

        public string Ask(string question=null, bool isYN = false, bool isCase=false)
        {
            return Aretha.Ask(question,isYN, Soul.Horus, isCase);
        }

        public string GetPath(string text, bool input, bool confirm = true)
        {
            return Aretha.GetPath(text, input, confirm, Soul.Horus);
        }
        public async void WaitForCommand(string[] args=null)
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

            try
            {
                switch (cmdlets[0])
                {
                    case "vigenere":
                    case "ceasar":
                    case "playfair":
                        await Classical(cmdlets[0],cmdlets[1]);
                        Aretha.SoulSucceeded(Soul.Anubis);
                        return;
                    case "cryptofile":
                        await CryptoFile(cmdlets[1]);
                        Aretha.SoulSucceeded(Soul.Anubis);
                        return;
                    case "enigma-machine":
                        await EnigmaMachine(cmdlets[1]);
                        Aretha.SoulSucceeded(Soul.Anubis);
                        return;
                    default:
                        throw new InvalidOperationException("Unknown operation");
                }
            }
            catch (Exception ex)
            {
                Aretha.SoulFailed(Soul.Horus, ex);
                return;
            }
        }

        private async Task CryptoFile(string cmd)
        {
            try
            {
                if (cmd == "create")
                {
                    var filePath = GetPath("Provide a path to the file you wish to encrypt", true);

                    var cryptoPath = GetPath("Provide path where the CryptoFile will be saved", false);

                    if (filePath == cryptoPath)
                    {
                        Speak("Input file path cannot be same as crypto file path");
                        await CryptoFile(cmd);
                        return;
                    }

                    var key = Ask("Provide the key to use:");

                    using var fsFile = File.OpenRead(filePath);
                    using var fsCrypt = File.OpenWrite(cryptoPath);


                    Speak("Executing Command. Please Wait");

                    using var file = new CryptoFile(fsCrypt, key);
                    await file.WriteAsync(fsFile, Aretha.WriteProgress());

                    Speak("Command Executed Successfully");
                    return;

                }
                else if (cmd == "open")
                {
                    var cryptoPath = GetPath("Provide a path to the CryptoFile", true);

                    var filePath = GetPath("Provide path to a file where the decrypted file should be saved", false);

                    if (filePath == cryptoPath)
                    {
                        Speak("Input file path cannot be same as output file path");
                        await CryptoFile(cmd);
                        return;
                    }

                    var key = Ask("Provide the key to use:");

                    using var fsFile = File.OpenRead(filePath);
                    using var fsCrypt = File.OpenWrite(cryptoPath);


                    Speak("Executing Command. Please Wait");

                    using var file = new CryptoFile(fsCrypt, key);
                    await file.ReadAsync(fsFile, Aretha.WriteProgress());

                    Speak("Command Executed Successfully");
                    return;
                }
            }
            catch
            {
                throw;
            }
        }

        private Task Classical(string algName, string cmd)
        {
            if (cmd != "encrypt" || cmd != "decrypt") 
                throw new InvalidOperationException($"{cmd} is not a valid classical algorithm command");

            try
            {
                ClassicalAlgorithm alg = algName switch
                {
                    "playfair" => new Playfair(),
                    "ceasar" => new Ceasar(),
                    "vigenere" => new Vigenere(),
                    _=>throw 
                    new InvalidOperationException("The classical algorithm provided is unknown")
                };

                var resp = Ask("Do you wish to use the default alphabet?", true);

                if (resp == "y")
                {
                    alg.Alphabet = algName == "playfair" ? Alphabets.ASCIIPerfect :
                        Alphabets.ASCII;
                }
                else
                {
                    resp = Ask("Would you like to use one of the predefined alphabets?", true);

                    if (resp == "y")
                    {
                        resp = "Enter the name of the predefined alphabet:";

                        alg.Alphabet = resp switch
                        {
                            "ascii" => Alphabets.ASCII,
                            "ascii-perfect" => Alphabets.ASCIIPerfect,
                            "codepage37" => Alphabets.CodePage437,
                            "large-alphabet" => Alphabets.LargeAlphabet,
                            "alphanumeric" => Alphabets.AlphaNumeric,
                            "uppercase" => Alphabets.EnglishCaps,
                            "lowercase" => Alphabets.EnglishSmall,
                            _ => throw new InvalidOperationException("Provided alphabet unknown")
                        };
                    }
                    else
                    {
                        Speak("Alphabet Capture ready, every letter you enter from now will be added to your custom alphabet. " +
                            "Press Enter/Escape to end Capture. Remember that word character order is important");

                        var alpha = new Alphabet();
                        while (true)
                        {
                            var character = Console.ReadKey();
                            if (character.Key == ConsoleKey.Enter || character.Key == ConsoleKey.Escape) break;

                            alpha.Add(character.KeyChar);
                        }
                        Console.WriteLine();
                        Speak($"Alphabet Capture Ended. Your custom alphabet has: {alpha.Count} characters");

                        alg.Alphabet = alpha;
                    }
                }

                if (algName == "playfair")
                {
                    resp = Ask("Do you want to auto-size your alphabet grid?", true);

                    if (resp == "y")
                    {
                        alg.Alphabet.AutoDimensions();
                    }
                    else
                    {
                        resp = Ask("Enter the size of the grid you want to use e.g 5x5 [lengthxwidth]");

                        var dimens = resp.Split('x');
                        alg.Alphabet.Dimensions = new Dimensions(int.Parse(dimens[0]), int.Parse(dimens[1]));
                    }
                }

                var key = Ask("Enter the Key", false, true);
                alg.Key = key;

                var input = Ask("Enter the input: ", false,true);

                var forward = cmd == "encrypt";

                var output = alg.Run(input, forward);

                Speak("Command Executed. Output provided below");

                Speak(output);

                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            
        }

        private Task EnigmaMachine(string cmd)
        {
            return Task.CompletedTask;
        }
    }
}
