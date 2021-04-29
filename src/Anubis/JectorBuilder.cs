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

        public JectorBuilder WithType(JectorType type)
        {
            _type = type;
            return this;
        }

        public JectorBuilder WithRequiredSuccess()
        {
            _success = true;
            return this;
        }

        public JectorBuilder WithEOF(string eof)
        {
            _eof = eof;
            return this;
        }

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
        Bitmap, WaveFile
    }
}