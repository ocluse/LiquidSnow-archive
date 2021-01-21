using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Thismaker.Anubis.Media
{
    public partial class WaveFile
    {
        public void Save(Stream stream)
        {
            //Set stream position to origin:
            stream.Position = 0;

            //RIFF Marker
            var buffer = "RIFF".GetBytes<ASCIIEncoding>();

            stream.Write(buffer, 0, buffer.Length);

            //ChunkSize:
            buffer = BitConverter.GetBytes(_chunkSize);
            stream.Write(buffer, 0, buffer.Length);

            //Formart Marker:
            buffer = "WAVE".GetBytes<ASCIIEncoding>();
            stream.Write(buffer, 0, buffer.Length);

            //-----------------------------FORMAT---------------------------------------
            buffer = "fmt ".GetBytes<ASCIIEncoding>();
            stream.Write(buffer, 0, buffer.Length);

            //fmt Chunk size
            buffer = BitConverter.GetBytes(_fmtChunkSize);
            stream.Write(buffer, 0, buffer.Length);

            //AudioFormat
            buffer = BitConverter.GetBytes(_audioFormat);
            stream.Write(buffer, 0, buffer.Length);

            //Channels
            buffer = BitConverter.GetBytes(NumChannels);
            stream.Write(buffer, 0, buffer.Length);

            //Sample Rate
            buffer = BitConverter.GetBytes(SampleRate);
            stream.Write(buffer, 0, buffer.Length);

            //Byte Rate
            buffer = BitConverter.GetBytes(ByteRate);
            stream.Write(buffer, 0, buffer.Length);

            //BlockAlign
            buffer = BitConverter.GetBytes(BlockAlign);
            stream.Write(buffer, 0, buffer.Length);

            //BitsPerSample
            buffer = BitConverter.GetBytes(BitsPerSample);
            stream.Write(buffer, 0, buffer.Length);

            //----------------------DATA MARKER----------------------------------------

            buffer = "data".GetBytes<ASCIIEncoding>();
            stream.Write(buffer, 0, buffer.Length);

            //Data chunk size
            buffer = BitConverter.GetBytes(_dataChunkSize);
            stream.Write(buffer, 0, buffer.Length);

            //write data:
            for(int i = 0; i < Channels[0].Samples.Count; i++)
            {
                foreach(var channel in Channels)
                {
                    buffer = channel.Samples[i].Data;
                    Array.Reverse(buffer);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}
