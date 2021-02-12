using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Thismaker.Horus.Classical
{
    public partial class EnigmaMachine
    {

        public void Save(Stream stream)
        {
            //header
            using var writer = new BinaryWriter(stream);

            //Header
            writer.Write("EGMC");

            //Alphabet
            writer.Write("alph");
            writer.Write(Alphabet.Count);
            writer.Write(Alphabet.ToString());

            //Whether we autoreset
            writer.Write(AutoReset);

            //Stator Indexing:
            writer.Write("ETW ");
            writer.Write("indx");
            writer.Write(Stator.Indexing.Count);
            writer.Write(Stator.Indexing.ToString());

            //Stator Wiring
            writer.Write("wire");
            writer.Write(Stator.Wiring.Count);
            writer.Write(Stator.Wiring.ToString());

            //Reflector Indexing
            writer.Write("UKW ");
            writer.Write("indx");
            writer.Write(Reflector.Indexing.Count);
            writer.Write(Reflector.Indexing.ToString());

            //Reflector Wiring
            writer.Write("wire");
            writer.Write(Reflector.Wiring.Count);
            writer.Write(Reflector.Wiring.ToString());

            //Rotors
            writer.Write("ROTS");
            writer.Write(Rotors.Count);

            foreach (var rotor in Rotors)
            {
                //Indexing
                writer.Write("indx");
                writer.Write(rotor.Indexing.Count);
                writer.Write(rotor.Indexing.ToString());

                //Wiring
                writer.Write("wire");
                writer.Write(rotor.Wiring.Count);
                writer.Write(rotor.Wiring.ToString());

                //Turnover Notch
                writer.Write("ntch");
                writer.Write(rotor.TurnOver.Count);
                foreach (var notch in rotor.TurnOver)
                {
                    writer.Write(notch);
                }
            }
        }

        public static EnigmaMachine Load(Stream stream)
        {

            using var reader = new BinaryReader(stream);

            //HEADER
            var buffer = reader.ReadString();
            if (buffer!="EGMC") throw new FormatException("Not a valid Enigma Machine File");


            //ALPHABET
            buffer = reader.ReadString();
            if (buffer!="alph") throw new FormatException("Not a valid Enigma Machine File");
            var len = reader.ReadInt32();
            buffer = reader.ReadString();
            var alphabet = new Alphabet(buffer);

            //AUTORESET
            var autroreset = reader.ReadBoolean();

            //STATOR AND REFLECTOR
            var list = new string[] { "ETW ", "UKW " };
            var wheels = new List<EnigmaWheel>(2);
            foreach(var item in list)
            {
                buffer = reader.ReadString();
                if (buffer!=item) throw new FormatException("Not a valid Enigma Machine File");
                buffer = reader.ReadString();
                if (buffer!="indx") throw new FormatException("Not a valid Enigma Machine File");
                len = reader.ReadInt32();
                buffer = reader.ReadString();
                var indexing = new Alphabet(buffer);

                buffer = reader.ReadString();
                if (buffer!="wire") throw new FormatException("Invalid/Corrupted Enigma File");
                len = reader.ReadInt32();
                buffer = reader.ReadString();
                var wiring = new Alphabet(buffer);

                var wheel = new EnigmaWheel(indexing, wiring);
                wheels.Add(wheel);
            }

            //ROTORS
            buffer = reader.ReadString();
            if (buffer!="ROTS") throw new FormatException("Invalid/Corrupted Enigma File");

            len = reader.ReadInt32();

            var builder = new StringBuilder();
            var rotors = new List<Rotor>();
            while(true)
            {
                builder.Clear();
                buffer = reader.ReadString();
                if (buffer!="indx") 
                    throw new FormatException("Invalid/Corrupted Enigma File");
                len = reader.ReadInt32();
                buffer = reader.ReadString();
                var indexing = new Alphabet(buffer);

                buffer = reader.ReadString();
                if (buffer!="wire") 
                    throw new FormatException("Invalid/Corrupted Enigma File");
                len = reader.ReadInt32();
                buffer = reader.ReadString();
                var wiring = new Alphabet(buffer);
                var wheel = new EnigmaWheel(indexing, wiring);

                buffer = reader.ReadString();
                if (buffer!="ntch")
                    throw new FormatException("Invalid/Corrupted Enigma File");

                len = reader.ReadInt32();
                for(int t = 0; t < len; t++)
                {
                    builder.Append(reader.ReadChar());
                }

                var rotor = new Rotor(indexing, wiring, builder.ToString());
                rotors.Add(rotor);

                if (reader.PeekChar() == -1) break;
            }

            //BUILDER
            return new EnigmaMachineBuilder()
                .WithAlphabet(alphabet)
                .WithRotors(rotors)
                .WithStator(wheels[0])
                .WithReflector(wheels[1])
                .Build();
        }
    }
}