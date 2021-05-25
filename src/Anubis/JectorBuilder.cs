using System;
using Thismaker.Anubis.Imaging;
using Thismaker.Anubis.Media;

namespace Thismaker.Anubis
{
    public class JectorBuilder
    {
        private JectorType? _type=null;
        private string _eof = null;
        private bool _success = false;

        /// <summary>
        /// The type of the Jector, i.e whether Image, or WaveFile
        /// </summary>
        public JectorBuilder WithType(JectorType type)
        {
            _type = type;
            return this;
        }

        /// <summary>
        /// If provided, then an End of File must be found while ejecting,
        /// also, the provided input must be able to fit the medium in it's entirety.
        /// </summary>
        public JectorBuilder WithRequiredSuccess()
        {
            _success = true;
            return this;
        }

        /// <summary>
        /// If provided, changes the EOF from the default value to the string provided.
        /// </summary>
        public JectorBuilder WithEOF(string eof)
        {
            _eof = eof;
            return this;
        }

        /// <summary>
        /// Constructs the Jector with the provided configuration
        /// </summary>
        /// <returns></returns>
        public Jector Build()
        {
            if (!_type.HasValue) throw new InvalidOperationException("Cannot build a typeless Jector");

            Jector result = _type.Value switch
            {
                JectorType.Bitmap => new BitmapJector(),
                JectorType.WaveFile=>new WaveFileJector(),
                _ => throw new InvalidOperationException("Unknown Jector Type")
            };

            if (!string.IsNullOrEmpty(_eof))
            {
                result.EOF = _eof;
            }

            result.EnsureSuccess = _success;

            return result;
        }
    }

    public enum JectorType
    {
        /// <summary>
        /// For hiding data in bitmap images, e.g PNG or JPEGs
        /// </summary>
        Bitmap, 

        /// <summary>
        /// For hiding data in audio WAV files
        /// </summary>
        WaveFile
    }
}