using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Horus.IO;

namespace Thismaker.Aretha
{
    class HorusContainer : HorusSoul, ISoulServant
    {
        //private CryptoContainer _container;
        //private FileStream _stream;

        private string _key;
        private string _path;
        private readonly List<string> GenCommands = new List<string>()
        {
            "add",
            "get",
            "delete",
            "extract"
        };

        private async Task OnCommand(string command, string args)
        {

            using var containerStream = new FileStream(_path, FileMode.OpenOrCreate);
            using var container = new CryptoContainer(containerStream, _key);
            switch (command)
            {
                case "add":
                case "get":
                    var opts = args.Split("path=", StringSplitOptions.TrimEntries);

                    var name = opts[0];
                    var path = opts[1];

                    path=path.Trim('"');

                    var get = command == "get";

                    var mode = get ? FileMode.Create : FileMode.Open;
                    var access = get ? FileAccess.Write : FileAccess.Read;

                    using (var stream = File.Open(path, mode, access))
                    {
                        
                        if (get)
                        {
                            
                            await container.GetAsync(name, stream, Aretha.WriteProgress());
                    }
                        else
                        {
                            await container.AddAsync(name, stream, true, Aretha.WriteProgress());
                        }
                    }          
                    return;
                
                case "delete":
                    var deleted=container.Delete(args);
                    if (deleted) Speak("File deleted successfully");
                    else Speak("Deletion failed.");
                    break;
                
                case "extract":
                    if (args.ToLower().StartsWith("path="))
                        args = args.Remove(0, "path=".Length);
                    args = args.Trim('"');
                    await container.ExtractContainerAsync(args, Aretha.WriteProgress());
                    break;
            }
        }

        public async Task OnSession(string args=null)
        {
            while (true)
            {
                var resp = Ask(null,false, true);

                if (resp.ToLower() == "back")
                {
                    return;
                }

                if (resp.ToLower().StartsWith("path="))
                {
                    _path = resp.Remove(0, "path=".Length);
                    _path.Trim('"');
                    continue;
                }
                else if (resp.ToLower().StartsWith("key="))
                {
                    _key= resp.Remove(0, "key=".Length);
                    continue;
                }

                if (string.IsNullOrEmpty(_path))
                {
                    Speak("Please provide a container path first");
                    continue;
                }

                if (string.IsNullOrEmpty(_key))
                {
                    Speak("Please provide a container key first");
                    continue;
                }

                bool valid = false;
                foreach(var command in GenCommands)
                {
                    if(resp.ToLower().StartsWith($"{command} "))
                    {
                        var ag = resp.Remove(0, command.Length + 1);

                        await OnCommand(command, ag);
                        valid = true;
                        break;
                    }
                }

                if (!valid) Speak("Not a valid command");
            }
        }
    }
}
