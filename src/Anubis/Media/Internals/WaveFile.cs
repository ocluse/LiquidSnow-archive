using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Thismaker.Anubis.Media
{
    internal class WaveFile : IDisposable
    {
        #region Private Fields

        private bool _hasHeader = false;

        private int
           _chunkSize,
           _fmtChunkSize,
           _dataChunkSize,
           _listChunkSize;

        private readonly Stream _stream;

        private WaveFormat _format;

        #endregion

        #region Properties

        public bool EnsureCleanRead { get; set; }

        public WaveFormat Format
        {
            get => _format;
        }

        public int DataSize
        {
            get => _dataChunkSize;
        }

        #endregion

        #region Constructors

        public WaveFile(Stream stream)
        {
            _stream = stream;

            if (stream.Length > 0)
            {
                LoadHeader();
            }
        }

        #endregion

        #region Private Methods

        private void LoadHeader()
        {
            _stream.Position = 0;
            using BinaryReader reader = new BinaryReader(_stream, Encoding.ASCII, true);

            //--------------------------------VALIDITY--------------------------------//
            //RIFF Marker
            char[] str = reader.ReadChars(4);
            if (!str.IsString("RIFF")) throw new InvalidDataException("The file is not a valid WAV file");

            //ChunkSize
            _chunkSize = reader.ReadInt32();

            //Formart Marker
            if (!reader.ReadChars(4).IsString("WAVE")) throw new InvalidDataException("The file is not a valid wave file");

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
            reader.BaseStream.Position += (_fmtChunkSize - 16);

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

        #endregion

        #region Public Methods

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
            foreach (Channel channel in sample.Channels)
            {
                if (channel.Data.Length != _format.BytesPerSample)
                    throw new InvalidDataException("Invalid channel size for WaveFile");

                writer.Write(channel.Data);
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

        public Sample GetNextSample()
        {
            using BinaryReader reader = new BinaryReader(_stream, Encoding.ASCII, true);

            Sample sample = new Sample();
            for (int i = 0; i < _format.NumChannels; i++)
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
                byte[] data = reader.ReadBytes(_format.BytesPerSample);
                Channel channel = new Channel(data);
                sample.Channels.Add(channel);
            }

            return sample;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        #endregion
    }
}