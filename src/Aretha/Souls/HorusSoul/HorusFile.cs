using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Thismaker.Horus.IO;

namespace Thismaker.Aretha
{
    class HorusFile : HorusSoul, ISoulServant
    {


        private List<string> GenCommands=new List<string>()
        {
            "encrypt",
            "decrypt"
        };

        private List<string> Args = new List<string>()
        {
            "key",
            "input", 
            "output"

        };



        public async Task OnSession(string args = null)
        {
            while (true)
            {
                var resp = Ask(null, false, true);

                if (resp.ToLower() == "back")
                {
                    return;
                }

                var valid = false;
                foreach(var command in GenCommands)
                {
                    
                    if(resp.ToLower().StartsWith($"{command} "))
                    {
                        string key=null, input=null, output = null;

                        var part = resp.Remove(0, command.Length + 1);

                        foreach(var arg in Args)
                        {
                            if (!part.ToLower().StartsWith($"-{arg} "))continue;

                            part = part[(arg.Length + 2)..];

                            var split = part.Split('"',StringSplitOptions.RemoveEmptyEntries);

                            if (nameof(key) == arg) key = split[0];
                            if (nameof(input) == arg) input = split[0];
                            if (nameof(output) == arg) output = split[0];

                            part = part[(split[0].Length+2)..].Trim();
                        }

                        if(string.IsNullOrEmpty(key)||
                            string.IsNullOrEmpty(input)||
                            string.IsNullOrEmpty(output))
                        {
                            break;
                        }

                        bool encrypt = command == "encrypt";

                        using var cryptoStream =  encrypt? File.OpenWrite(output):File.OpenRead(input);
                        using var plainStream = encrypt? File.OpenRead(input):File.OpenWrite(output);
                        using var file = new CryptoFile(cryptoStream, key);

                        if (encrypt) await file.WriteAsync(plainStream, Aretha.WriteProgress());
                        else await file.ReadAsync(plainStream, Aretha.WriteProgress());

                        valid = true;
                        break;
                    }

                    if (!valid) Speak("Not a valid command");
                }
            }
        }
    }
}
