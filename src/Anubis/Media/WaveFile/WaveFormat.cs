namespace Thismaker.Anubis.Media
{
    public struct WaveFormat
    {
        /// <summary>
        /// If 1, then it means PCM
        /// </summary>
        public short AudioFormat { get; set; }

        /// <summary>
        /// The number of audio channels. 1-Mono, 2-Stereo
        /// </summary>
        public short NumChannels { get; set; }

        /// <summary>
        /// The sample rate of the audio, e.g 44100Hz etc
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// The bits per sample, e.g 8bits, 16bits etc
        /// </summary>
        public short BitsPerSample { get; set; }

        /// <summary>
        /// The byte rate. Don't know what it is in detail just that :)
        /// </summary>
        public int ByteRate
        {
            get { return SampleRate * NumChannels * BitsPerSample / 8; }
        }

        public int BytesPerSample
        {
            get { return BitsPerSample / 8; }
        }

        /// <summary>
        /// The number of bytes fr one sample including all channels.
        /// </summary>
        public short BlockAlign
        {
            get { return (short)(NumChannels * BitsPerSample / 8); }
        }
    }
}
