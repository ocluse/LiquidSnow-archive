using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Thismaker.Anubis.Media
{
    public partial class WaveFile
    {
        private void LoadHeader()
        {
            _stream.Position = 0;
            using BinaryReader reader = new BinaryReader(_stream, Encoding.ASCII, true);

            //--------------------------------VALIDITY--------------------------------//
            //RIFF Marker
            char[] str =reader.ReadChars(4);
            if(!str.IsString("RIFF")) throw new InvalidDataException("The file is not a valid WAV file");

            //ChunkSize
            _chunkSize = reader.ReadInt32();

            //Formart Marker
            if(!reader.ReadChars(4).IsString("WAVE")) throw new InvalidDataException("The file is not a valid wave file");

            //----------------------------DATA 1-------------------------------------//
            if (!reader.ReadChars(4).IsString("fmt ")) throw new InvalidDataException("Invalid or unrecognized WAV file");

            //fmt Chunk size
            _fmtChunkSize = reader.ReadInt32();

            //AudioFormat
            _format.AudioFormat = reader.ReadInt16();

            //Channels
            _format.NumChannels = reader.ReadInt16();

            //Sample Rate
            _format.SampleRate = reader.ReadInt32();

            //theoritical ByteRate
            int byteRate = reader.ReadInt32();

            //theoritical BlockAlign
            short blockAlign = reader.ReadInt16();

            //Theoritical BitsPerSample
            _format.BitsPerSample = reader.ReadInt16();

            //Check for EnsureCleanRead:
            if ((byteRate != _format.ByteRate ||
                blockAlign != _format.BlockAlign) && EnsureCleanRead)
                throw new InvalidDataException("There is an error in the WAV file");

            //Advance streamPostion:
            reader.BaseStream.Position+= (_fmtChunkSize - 16);

            //==========================DATA 2============================//
            string strBuffer = new string(reader.ReadChars(4));

            if (strBuffer != "data")
            {
                if (strBuffer == "LIST")
                {
                    _listChunkSize = reader.ReadInt32();

                    //Advance position by chunksize"
                    reader.BaseStream.Position += _listChunkSize;

                    //try to find data:

                    if (!reader.ReadChars(4).IsString("data")) throw new InvalidDataException("Invalid/unknown WAV file");
                }
                else
                {
                    throw new InvalidDataException("Invalid/unknown WAV file");
                }
            }

            //Data chunk size
            _dataChunkSize = reader.ReadInt32();

            _hasHeader = true;
        }

        public Sample GetNextSample()
        {
            using BinaryReader reader = new BinaryReader(_stream, Encoding.ASCII, true);

            Sample sample = new Sample();
            for(int i = 0; i < _format.NumChannels; i++)
            {
                if (reader.PeekChar() == -1)
                {
                    if (sample.Channels.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return sample;
                    }
                }
                byte[] data =reader.ReadBytes(_format.BytesPerSample);
                Channel channel = new Channel(data);
                sample.Channels.Add(channel);
            }

            return sample;
        }
    }

    public class Sample
    {
        public List<Channel> Channels { get; set; }
         = new List<Channel>();
    }

    public class Channel
    {
        public byte[] Data { get; set; }

        public Channel(byte[] data)
        {
            Data = data;
        }
    }

    
}
