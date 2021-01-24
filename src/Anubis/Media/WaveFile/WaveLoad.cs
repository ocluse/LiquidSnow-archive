using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Thismaker.Anubis.Media
{
    public partial class WaveFile
    {
       

        public void Load(Stream stream)
        {
            stream.Position = 0;
            //--------------------------------VALIDITY--------------------------------//
            //RIFF Marker
            var buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            var strBuffer=buffer.GetString<ASCIIEncoding>();
            if (strBuffer != "RIFF") throw new InvalidDataException("The file is not a valid WAV file");

            //ChunkSize
            stream.Read(buffer, 0, buffer.Length);
            _chunkSize = BitConverter.ToInt32(buffer, 0);

            //Formart Marker
            stream.Read(buffer, 0, buffer.Length);
            strBuffer = buffer.GetString<ASCIIEncoding>();
            if (strBuffer != "WAVE") throw new InvalidDataException("The file is not a valid wave file");


            //----------------------------DATA 1-------------------------------------//

            stream.Read(buffer, 0, buffer.Length);
            strBuffer = buffer.GetString<ASCIIEncoding>();

            if (strBuffer != "fmt ") throw new InvalidDataException("Invalid or unrecognized WAV file");

            //fmt Chunk size
            stream.Read(buffer, 0, buffer.Length);
            _fmtChunkSize = BitConverter.ToInt32(buffer, 0);

            //AudioFormat
            buffer = new byte[2];
            stream.Read(buffer, 0, buffer.Length);
            _audioFormat = BitConverter.ToInt16(buffer, 0);
            
            //Channels
            buffer = new byte[2];
            stream.Read(buffer, 0, buffer.Length);
            NumChannels = BitConverter.ToInt16(buffer, 0);

            //Sample Rate
            buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            SampleRate = BitConverter.ToInt32(buffer, 0);

            //theoritical ByteRate
            stream.Read(buffer, 0, buffer.Length);
            var byteRate = BitConverter.ToInt32(buffer, 0);

            //theoritical BlockAlign
            buffer = new byte[2];
            stream.Read(buffer, 0, buffer.Length);
            var blockAlign = BitConverter.ToInt16(buffer, 0);

            //Theoritical BitsPerSample
            buffer = new byte[2];
            stream.Read(buffer, 0, buffer.Length);
            BitsPerSample = BitConverter.ToInt16(buffer, 0);

            //Check for EnsureCleanRead:
            if ((byteRate != ByteRate ||
                blockAlign != BlockAlign) && EnsureCleanRead) 
                throw new InvalidDataException("There is an error in the WAV file");

            //Advance streamPostion:
            
            stream.Position +=( _fmtChunkSize - 16);

            //==========================DATA 2============================//
            buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            strBuffer = buffer.GetString<ASCIIEncoding>();

            if (strBuffer == "data")
            {
                GetData(stream);
            }
            else if (strBuffer == "LIST")
            {
                stream.Read(buffer, 0, buffer.Length);
                _listChunkSize = BitConverter.ToInt32(buffer, 0);

                //Advance position by chunksize"
                stream.Position += _listChunkSize;

                //try to find data:
                buffer = new byte[4];
                stream.Read(buffer, 0, buffer.Length);
                strBuffer = buffer.GetString<ASCIIEncoding>();

                if (strBuffer != "data") throw new InvalidDataException("Invalid/unknown WAV file");
                GetData(stream);
            }
            else
            {
                throw new InvalidDataException("Invalid/unknown WAV file");
            }
        }

        

        private void GetData(Stream stream)
        {
            //Data chunk size
            var buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            _dataChunkSize = BitConverter.ToInt32(buffer, 0);

            //Actual Data
            
            Channels = new List<ChannelData>();

            //Create the channels:
            for(int i = 0; i < NumChannels; i++)
            {
                Channels.Add(new ChannelData());
            }

            int channelIndex = 0;

            buffer = new byte[BitsPerSample/8];
            
            while (true)
            {
                if (channelIndex >= NumChannels) channelIndex = 0;

                //read the audio data:
                stream.Read(buffer, 0, buffer.Length);

                Sample sample = new Sample() { Data = buffer };

                Channels[channelIndex].Samples.Add(sample);

                channelIndex++;

                if (stream.Position >= stream.Length-1)
                {
                    break;
                }
            }
        }
    }



    public struct Sample
    {
        public byte[] Data { get; set; }
    }

    public class ChannelData
    {
        public List<Sample> Samples { get; set; } = new List<Sample>();
    }
}
