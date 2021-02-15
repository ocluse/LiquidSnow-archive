using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Horus.Classical;
using System.IO;

namespace Thismaker.Aretha
{
    class HorusEnigma : HorusSoul, ISoulServant
    {
        EnigmaMachine _enigma;

        public List<string> ArgCommands = new List<string>
        {
            "load",
            "save",
            "run",
            "key",
            "autoreset",
        };

        public List<string> SoloCommands = new List<string>
        {
            "create",
            "reset",
        };

        public Task OnSession(string args=null)
        {
            while (true)
            {
                var resp = Ask(null, false, true);

                if (resp.ToLower() == "back") return Task.CompletedTask;

                var valid = false;
                foreach(var command in SoloCommands)
                {
                    if (resp.ToLower() == command)
                    {
                        OnCommand(command, null);
                        valid = true;
                    }
                }

                foreach(var command in ArgCommands)
                {
                    if (resp.ToLower().StartsWith($"{command} "))
                    {
                        args = resp[(command.Length + 1)..];

                        OnCommand(command, args);
                        valid = true;
                    }
                }

                if (!valid)
                {
                    Speak("Not a valid command");
                }

            }
        }

        private void OnCommand(string command, string args)
        {
            switch (command) 
            {
                case "load":
                case "save":
                    var load = command == "load";
                    args = args.Trim('"');
                    using (var fs = load ? File.OpenRead(args) : File.OpenWrite(args))
                    {
                        if (load)
                        {
                            _enigma = EnigmaMachine.Load(fs);
                        }
                        else
                        {
                            if (_enigma == null) 
                            {
                                Speak("No machine has been created or loaded!"); 
                                return; 
                            }
                            _enigma.Save(fs);
                        }
                    }
                    break;

                case "create":
                    _enigma = new EnigmaMachineBuilder().Build();
                    break;

                case "run":

                    if (_enigma == null)
                    {
                        Speak("No machine has been created or loaded!");
                        return;
                    }

                    Speak(_enigma.Run(args, true));
                    break;

                case "key":

                    if (_enigma == null)
                    {
                        Speak("No machine has been created or loaded!");
                        return;
                    }

                    _enigma.Key = args;
                    _enigma.ResetRotors();
                    break;
                
                case "reset":
                    if (_enigma == null)
                    {
                        Speak("No machine has been created or loaded!");
                        return;
                    }
                    _enigma.ResetRotors();
                    break;
                case "autoreset":
                    if(args.ToLower()!="false"&& args.ToLower() != "true")
                    {
                        Speak("Invalid Auto Reset Value. Accepted values are true and false.");
                        return;
                    }
                    var on = args.ToLower() == "true";
                    if (_enigma == null)
                    {
                        Speak("No machine has been created or loaded!");
                        return;
                    }
                    _enigma.AutoReset = on;
                    break;
            }
        }

    }
}
