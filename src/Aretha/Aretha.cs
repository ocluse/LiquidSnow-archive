using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Thismaker.Aretha
{
    static class Aretha
    {
        static TaskCompletionSource tcsSummon;
        static async Task Main(string[] args)
        {
            await Prepare(args);
        }

        private static async Task Prepare(string[] args)
        {
            Console.ResetColor();
            if (args != null && args.Length > 0)
            {
                await Act(args);
                return;
            }

            var instruction = Ask("Ready to summon");

            if (instruction == null) return;

            await Act(instruction.Split(' '));
        }

        static async Task Act(string[] args)
        {
            var left_args = new List<string>();

            if (args != null)
            {
                for (int i = 2; i < args.Length; i++)
                {
                    left_args.Add(args[i]);
                }
            }

            if (args[0] == "summon")
            {
                if (args[1] == "abilities" && args.Length==2)
                {
                    Speak("You can currently summon the following entities: \n\t1.Anubis\n\t2.Enigma");
                }
                else
                {
                    try
                    {
                        var soul = SoulFromName(args[1]);
                        await Summon(soul, left_args.ToArray());
                    }
                    catch (UnknownSoulException)
                    {
                        Speak($"{args[1]} is not summonable. To see what is summonable, type summon abilities");
                    }
                }
            }
            else
            {
                Speak("Command not recognized");
            }

            await Prepare(null);
        }

        static async Task Summon(Soul soul, string[] args=null)
        {
            try
            {
                ISoul soulHost;
                soulHost = soul switch
                {
                    Soul.Anubis => new AnubisSoul(),
                    Soul.Enigma => new EnigmaSoul(),
                    _ => throw new UnknownSoulException()
                };
                tcsSummon = new TaskCompletionSource();
                Speak($"{soul} has been summoned");
                soulHost.WaitForCommand(args);
                await tcsSummon.Task;
            }
            catch (UnknownSoulException)
            {
                Speak("Soul is not known or cannot be summoned");
            }
            catch(TaskCanceledException)
            {
                tcsSummon = null;
                var response=Ask($"{soul} dismissed. Do you want to summon {soul} again? (Y/N)", true);
                
                if (response == "y")
                {
                    await Summon(soul);
                }
            }
            catch (SummonFailedException ex)
            {
                Speak($"{soul} was unable to executed your command: {ex.Message}");
                var response = Ask( $"Do you want to summon {soul} again? (Y/N)");

                if (response == "y")
                {
                    await Summon(soul);
                }
            }

        }

        public static void SoulFailed(Soul soul, Exception ex)
        {
            tcsSummon.TrySetException(new SummonFailedException(soul,ex.Message));
        }

        public static void SoulSucceeded(Soul soul)
        {
            tcsSummon.TrySetResult();
        }

        public static ConsoleColor SoulColor(Soul soul)
        {
            return soul switch
            {
                Soul.Aretha => ConsoleColor.Yellow,
                Soul.Anubis => ConsoleColor.Cyan,
                Soul.Enigma=>ConsoleColor.Green,
                _=>ConsoleColor.White
            };
        }

        public static Soul SoulFromName(string name)
        {
            return name switch
            {
                "aretha" => Soul.Aretha,
                "anubis" => Soul.Anubis,
                "enigma" => Soul.Enigma,
                _=>throw new UnknownSoulException()
            };
        }

        public static void Speak(string text, Soul soul= Soul.Aretha)
        {
            Console.ForegroundColor = SoulColor(soul);
            Console.Write($"{soul}: ");
            Console.ResetColor();
            Console.Write(text);
            Console.WriteLine();
        }

        public static string Ask(string question=null, bool isYN = false, Soul soul=Soul.Aretha, bool isCase=false)
        {
            if (!string.IsNullOrEmpty(question))
            {
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

                if (args[0] == "dismiss" && args.Length==1)
                {
                    tcsSummon.TrySetCanceled();
                    cancelled = true;
                }
                else if (args[0] == "exit" && args.Length == 1)
                {
                    Environment.Exit(0);
                }
                else if(soul!=Soul.Aretha)
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

        public static string GetPath(string text, bool input, bool confirm = true, Soul soul=Soul.Aretha)
        {
            var response=Ask(text, false, soul, true);

            var path = response.Trim();
            path = path.Trim('\"');
            if (confirm)
            {
                if (input && !File.Exists(path))
                {
                    Speak("The file you have provided does not exist!",soul);
                    return GetPath(text, input, confirm, soul);
                }
                else if (!input && File.Exists(path))
                {
                    Speak("This file already exists. Are you sure you want to overwrite it? (Y/N)", soul);
                    
                    response = Ask(text,true, soul);

                    if (response != "y") return GetPath(text, input, confirm, soul);
                }
            }
            return path;
        }

    }

    enum Soul {Aretha, Anubis, Enigma }

    public class UnknownSoulException:Exception
    {
        public UnknownSoulException(string msg) : base(msg)
        {

        }

        public UnknownSoulException() : base() { }
    }

    internal class SummonFailedException:Exception
    {
        public Soul Soul { get; set; }
        public SummonFailedException(Soul soul,string msg):base(msg)
        {
            Soul = soul;
        }
    }

}
