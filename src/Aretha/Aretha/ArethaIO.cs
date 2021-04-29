using System;
using System.IO;
using System.Threading.Tasks;
using Thismaker.Core;

namespace Thismaker.Aretha
{
    public static partial class Aretha
    {
        public static Soul SoulFromName(string name)
        {
            return name switch
            {
                "aretha" => Soul.Aretha,
                "anubis" => Soul.Anubis,
                "horus" => Soul.Horus,
                _ => throw new UnknownSoulException()
            };
        }

        public static void Speak(string text, Soul soul = Soul.Aretha)
        {
            Console.ForegroundColor = SoulColor(soul);
            Console.Write($"{soul}: ");
            Console.ResetColor();
            Console.Write(text);
            Console.WriteLine();
        }

        public static string Ask(string question = null, bool isYN = false, Soul soul = Soul.Aretha, bool isCase = false)
        {
            if (!string.IsNullOrEmpty(question))
            {
                if (isYN && !question.EndsWith(" [Y/N]")) question += " [Y/N]";
                Speak(question, soul);
            }

            Console.ForegroundColor = SoulColor(soul);
            Console.Write(">> ");
            Console.ResetColor();

            var instruction = Console.ReadLine();
            var lower = instruction.ToLower();
            var cancelled = false;
            if (lower.StartsWith("@aretha ") || soul == Soul.Aretha)
            {
                var args = lower.Replace("@aretha ", null).Split(' ');

                if (args.Length == 0)
                {
                    Speak("Command cannot be empty");
                    return Ask(question, isYN, soul, isCase);
                }

                if (args[0] == "dismiss" && args.Length == 1)
                {
                    if (tcsSummon == null)
                    {
                        Speak("Command not available at this time. Please try again!");
                        return Ask(question, isYN, soul, isCase);
                    }
                    cancelled = true;
                    tcsSummon.TrySetCanceled();
                    throw new TaskCanceledException();
                    
                    
                }
                else if (args[0] == "exit" && args.Length == 1)
                {
                    Environment.Exit(0);
                }
                else if (soul != Soul.Aretha)
                {
                    Speak("Unknown Aretha command or command cannot be executed at the current time.");
                    return Ask(question, isYN, soul, isCase);
                }

            }
            if (isYN)
            {

                if (lower != "y" && lower != "n" && !cancelled)
                {
                    Speak("This response requires a Y (For Yes) or N (For No). Try again.");
                    return Ask(question, isYN, soul, isCase);
                }
            }

            return isCase ? instruction : lower;
        }

        public static string GetPath(string text, bool input, bool confirm = true, Soul soul = Soul.Aretha)
        {
            var response = Ask(text, false, soul, true);

            var path = response.Trim();
            path = path.Trim('\"');
            if (confirm)
            {
                if (input && !File.Exists(path))
                {
                    Speak("The file you have provided does not exist!", soul);
                    return GetPath(text, input, confirm, soul);
                }
                else if (!input && File.Exists(path))
                {
                    response = Ask("This file already exists. Are you sure you want to overwrite it?", true, soul);

                    if (response != "y") return GetPath(text, input, confirm, soul);
                }
            }
            return path;
        }

        public static Progress<float> WriteProgress()
        {
            var progress = new Progress<float>();

            int previousProgress = 0;

            DeltaTime.Capture();

            Console.WriteLine();

            progress.ProgressChanged += (o, e) =>
            {
                var er = Math.Round(e, 2);
                if (DeltaTime.Elapsed < 100 && er != 1) return;

                DeltaTime.Capture();
                int percent = (int)(er * 100.0f);

                if (percent - previousProgress < 1) return;

                previousProgress = percent;

                
                string progBar = "Execution Progress: ";
                for (int i = 0; i <= 100; i += 10)
                {
                    if (i <= percent) progBar += "|";
                    else progBar += ".";
                }
                Console.Write("\r");
                progBar += $" {percent}%";

                Console.Write(progBar);

                if (percent == 100)
                {
                    Console.WriteLine();
                }
            };

            return progress;
        }
    }
}
