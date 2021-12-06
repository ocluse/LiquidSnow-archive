using System.IO;
using System.Text;

namespace Thismaker.Anubis.Media
{
    public partial class WaveFile
    {
        public void SaveHeader()
        {
            //Set stream position to origin:
            _stream.Position = 0;
            using BinaryWriter writer = new BinaryWriter(_stream, Encoding.ASCII, true);

            //RIFF Marker
            writer.Write("RIFF".ToCharArray());

            //ChunkSize:
            writer.Write(_chunkSize);

            //Formart Marker:
            writer.Write("WAVE".ToCharArray());

            //-----------------------------FORMAT---------------------------------------
            writer.Write("fmt ".ToCharArray());

            //fmt Chunk size
            writer.Write(_fmtChunkSize);

            //AudioFormat
            writer.Write(_format.AudioFormat);

            //Channels
            writer.Write(_format.NumChannels);

            //Sample Rate
            writer.Write(_format.SampleRate);

            //Byte Rate
            writer.Write(_format.ByteRate);

            //BlockAlign
            writer.Write(_format.BlockAlign);

            //BitsPerSample
            writer.Write(_format.BitsPerSample);

            //----------------------DATA MARKER----------------------------------------
            writer.Write("data".ToCharArray());

            //Data chunk size
            writer.Write(_dataChunkSize);

            _hasHeader = true;
        }

        public void WriteSample(Sample sample)
        {
            if (!_hasHeader) SaveHeader();

            if (sample.Channels.Count != _format.NumChannels)
                throw new InvalidDataException("Sample channels must be equal to WaveFile channel count");

            
            using BinaryWriter writer = new BinaryWriter(_stream, Encoding.ASCII, true);
            foreach(Channel channel in sample.Channels)
            {
                if (channel.Data.Length != _format.BytesPerSample)
                    throw new InvalidDataException("Invalid channel size for WaveFile");

                writer.Write(channel.Data);
            }
        }
    }
}
