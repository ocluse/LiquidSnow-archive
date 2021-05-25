using System;

namespace Thismaker.Anubis
{
    /// <summary>
    /// An exception that is typically thrown by a <see cref="Jector"/>
    /// whenever an end of file related error occurs, for example, when the end of file is not found
    /// but was required.
    /// </summary>
    public class EndOfFileException : Exception
    {
        public EndOfFileException() : base() { }

        public EndOfFileException(string message) : base(message) { }

        public EndOfFileException(string message, Exception innerException) : base(message, innerException) { }
    }
}
