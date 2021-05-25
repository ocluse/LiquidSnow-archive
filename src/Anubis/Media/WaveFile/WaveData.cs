using System;
using System.IO;

namespace Thismaker.Anubis.Media
{
    /// <summary>
    /// This class is typically used by the <see cref="WaveFileJector"/> to read the wavefile data
    /// of a WAV file, though it can be easily modified to provide the data to a player, 
    /// if you know what you are doing.
    /// </summary>
    public partial class WaveFile : IDisposable
    {
        private bool _hasHeader = false;

        private int
           _chunkSize,
           _fmtChunkSize,
           _dataChunkSize,
           _listChunkSize;

        private readonly Stream _stream;

        /// <summary>
        /// Throws errors if the file is not cleanly intact
        /// </summary>
        public bool EnsureCleanRead { get; set; }

        private WaveFormat _format;

        public WaveFormat Format
        {
            get => _format;
        }

        public int DataSize
        {
            get => _dataChunkSize;
        }

        public WaveFile(Stream stream)
        {

            _stream = stream;

            if (stream.Length > 0)
            {
                LoadHeader();
            }
        }

        public void DuplicateFormat(WaveFile source)
        {
            _format = source.Format;
            _chunkSize = source._chunkSize;
            _fmtChunkSize = source._fmtChunkSize;
            _dataChunkSize = source._dataChunkSize;
            _listChunkSize = source._listChunkSize;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
