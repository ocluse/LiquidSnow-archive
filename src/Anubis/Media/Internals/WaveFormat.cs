namespace Thismaker.Anubis.Media
{
    internal struct WaveFormat
    {
        public short AudioFormat { get; set; }

        public short NumChannels { get; set; }

        public int SampleRate { get; set; }

        public short BitsPerSample { get; set; }

        public int ByteRate
        {
            get { return SampleRate * NumChannels * BitsPerSample / 8; }
        }

        public int BytesPerSample
        {
            get { return BitsPerSample / 8; }
        }

        public short BlockAlign
        {
            get { return (short)(NumChannels * BitsPerSample / 8); }
        }
    }
}
