using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thismaker.Aretha
{
    partial class HorusSoul:ISoul
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
            
            string cmd;
            if (args == null || args.Length == 0)
            {
                cmd = Ask();
                if (cmd == null) return;

                cmdlets = new List<string>(cmd.Split(' '));
            }
            else
            {
                cmdlets = new List<string>(args);
                StringBuilder sb = new();

                for (int i = 0; i < cmdlets.Count; i++){
                    sb.Append(cmdlets[i]);

                    if (i != cmdlets.Count - 1)
                    {
                        sb.Append(' ');
                    }
                }

                cmd = sb.ToString();
            }

            while (true)
            {
                try
                {
                    if (cmdlets[0] == "back")
                    {
                        Aretha.SoulSucceeded(Soul.Horus);
                        return;
                    }
                    if (cmdlets[0] == "use")
                    {
                        switch (cmdlets[1])
                        {
                            case "vigenere":
                            case "caesar":
                            case "playfair":
                                await new HorusClassical().OnSession(cmdlets[1]);
                                break;
                            case "crypto-file":
                                await new HorusFile().OnSession();
                                break;
                            case "crypto-container":
                                await new HorusContainer().OnSession();
                                break;
                            case "enigma-machine":
                                await new HorusEnigma().OnSession();
                                break;
                            default:
                                throw new InvalidOperationException("Unknown operation");
                        }
                    }
                    if (cmdlets[0] == "hash")
                    {
                        var input = Ask(isCase: true);
                        Speak(Horus.Horus.GetHashString(input));
                    }

                    cmdlets = new List<string>(Ask().Split(' '));
                }
                catch (Exception ex)
                {
                    Aretha.SoulFailed(Soul.Horus, ex);
                    return;
                }
            }
        }
    }
}
