using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thismaker.Horus.Classical;

namespace Thismaker.Aretha
{
    class HorusClassical : HorusSoul, ISoulServant
    {
        private ClassicalAlgorithm _alg;

        private void SetDimensions(string particular)
        {
            var dimens=particular.Split('x');
            _alg.Alphabet.Dimensions=new Dimensions(int.Parse(dimens[0]), int.Parse(dimens[1]));
        }

        private Alphabet AlphabetCapture()
        {
            Speak("Alphabet Capture ready, every letter you enter from now will be added to your custom alphabet. " +
                           "Press Enter/Escape to end Capture. Remember that word character order is important. Repeats will be ignored");

            var alpha = new Alphabet();
            while (true)
            {
                var character = Console.ReadKey(true);
                if (character.Key == ConsoleKey.Enter || character.Key == ConsoleKey.Escape) break;

                var added=alpha.Add(character.KeyChar);
                if (added) Console.Write(character.KeyChar);
            }
            Console.WriteLine();
            Speak($"Alphabet Capture ended with {alpha.Count} characters.");

            return alpha;
        }

        private void SetAlphabet(string particular)
        {
            if (particular == "custom")
            {
                _alg.Alphabet = AlphabetCapture();

            }
            else

            _alg.Alphabet = particular.ToLower() switch
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

        private readonly List<string> GetSetCommands = new List<string>()
        {
            "alphabet",
            "grid",
            "key"
        };

        private readonly List<string> GenCommands = new List<string>()
        {
            "get",
            "set",
            "encrypt",
            "decrypt"
        };

        public void OnRunCommand(bool forward, string args)
        {
            Speak(_alg.Run(args, forward));
        }

        public void OnGetSetCommand(string command, bool get =true,string args=null)
        {


            switch (command)
            {
                case "alphabet":
                    if (get) Speak(_alg.Alphabet.ToString()); 
                    else SetAlphabet(args);
                    return;
                case "grid":
                    if (get) Speak(_alg.Alphabet.Dimensions.ToString());
                    else SetDimensions(args.ToLower());
                    return;
                case "key":
                    if (get) Speak(_alg.Key);
                    else _alg.Key = args;
                    return;
            }
        }

        public void OnGenCommand(string command, string args)
        {
            switch (command)
            {
                case "set":
                case "get":
                    //var particular = args.Remove(0, "set ".Length);
                    var get = command == "get";
                    foreach (var cmdlet in GetSetCommands)
                    {
                        var test = get ? cmdlet : $"{cmdlet} ";
                        if (args.ToLower().StartsWith(test))
                        {
                            args = args.Remove(0, test.Length);
                            OnGetSetCommand(cmdlet, get, args);
                            break;
                        }
                        
                    }
                    return;
                case "encrypt":
                case "decrypt":
                    var forward = command == "encrypt";
                    OnRunCommand(forward, args);
                    return;
            }
        }

        public Task OnSession(string algName)
        {
            //set up the alg
            _alg = algName switch
            {
                "playfair" => new Playfair(),
                "caesar" => new Caesar(),
                "vigenere" => new Vigenere(),
                _ => throw
                new InvalidOperationException("The classical algorithm provided is unknown")
            };

            _alg.Alphabet = algName == "playfair" ? Alphabets.ASCIIPerfect :
                        Alphabets.ASCII;

            while (true)
            {
                var resp = Ask(null, false, true);

                if (resp.ToLower() == "back")
                {
                    return Task.CompletedTask;
                }

                bool valid = false;
                foreach (var gen in GenCommands)
                {
                    var args = resp.Remove(0, gen.Length + 1);
                    if(resp.ToLower().StartsWith($"{gen} "))
                    {
                        OnGenCommand(gen, args);
                        valid = true;
                        break;
                    }
                }
                if (!valid) Speak("Not a valid command");
            }
        }
    }
}
