using System;

namespace Thismaker.Anubis
{
    class EndOfFileException:Exception
    {
        public EndOfFileException() : base() { }

        public EndOfFileException(string message) : base(message) { }
    }
}
