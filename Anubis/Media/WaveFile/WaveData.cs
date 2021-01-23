using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Thismaker.Anubis.Media
{
    public partial class WaveFile
    {
        private int
           _chunkSize,
           _fmtChunkSize,
           _dataChunkSize,
           _listChunkSize;
        private short _audioFormat;

        /// <summary>
        /// Throws errors if the file is not cleanly intact
        /// </summary>
        public bool EnsureCleanRead { get; set; }

        /// <summary>
        /// The number of audio channels. 1-Mono, 2-Stereo
        /// </summary>
        public short NumChannels { get; private set; }

        /// <summary>
        /// The sample rate of the audio, e.g 44100Hz etc
        /// </summary>
        public int SampleRate { get; private set; }

        /// <summary>
        /// The bits per sample, e.g 8bits, 16bits etc
        /// </summary>
        public short BitsPerSample { get; private set; }

        /// <summary>
        /// The byte rate. Don't know what it is in detail just that :)
        /// </summary>
        public int ByteRate
        {
            get { return SampleRate * NumChannels * BitsPerSample / 8; }
        }

        /// <summary>
        /// The number of bytes fr one sample including all channels.
        /// </summary>
        public short BlockAlign
        {
            get { return (short)(NumChannels * BitsPerSample / 8); }
        }

        public List<ChannelData> Channels { get; set; }

        public WaveFile(Stream stream)
        {
            Load(stream);
        }
    }

    public struct WaveFormat
    {

    }
}
