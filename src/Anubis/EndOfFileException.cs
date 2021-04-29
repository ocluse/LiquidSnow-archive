using System;

namespace Thismaker.Anubis
{
    public class EndOfFileException : Exception
    {
        public EndOfFileException() : base() { }

        public EndOfFileException(string message) : base(message) { }
    }
}
