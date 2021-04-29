using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thismaker.Aretha
{
    public static partial class Aretha
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

            for (int i = 2; i < args.Length; i++)
            {
                left_args.Add(args[i]);
            }

            if (args[0] == "summon" && args.Length>1)
            {
                if (args[1] == "abilities")
                {
                    if (args.Length == 2)
                        Speak("You can currently summon the following entities: \n\t1.Anubis\n\t2.Horus");

                    else Speak("Command not recognized");
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
                ISoul soulHost = soul switch
                {
                    Soul.Anubis => new AnubisSoul(),
                    Soul.Horus => new HorusSoul(),
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
                var response=Ask($"{soul} dismissed. Do you want to summon {soul} again?", true);
                
                if (response == "y")
                {
                    await Summon(soul);
                }
            }
            catch (SummonFailedException ex)
            {
                Speak($"{soul} was unable to executed your command: {ex.Message}");
                var response = Ask( $"Do you want to summon {soul} again?", true);

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
                Soul.Horus=>ConsoleColor.Green,
                _=>ConsoleColor.White
            };
        }
    }

    public enum Soul {Aretha, Anubis, Horus }

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
