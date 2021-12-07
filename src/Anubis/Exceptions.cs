using System;
using System.IO;

namespace Thismaker.Anubis
{
    /// <summary>
    /// Represents an error that occurs when an end of file cannot be found
    /// </summary>
    [Serializable]
    public class EndOfFileException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EndOfFileException"/>
        /// </summary>
        public EndOfFileException() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="EndOfFileException"/> with the specified error message
        /// </summary>
        /// <param name="message"></param>
        public EndOfFileException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of <see cref="EndOfFileException"/> with the specified error message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public EndOfFileException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Represents an error that occurs when a <see cref="Jector"/> cannot entirely write data to an input media source
    /// </summary>
    [Serializable]
    public class InsufficientSpaceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="InsufficientSpaceException"/>
        /// </summary>
        public InsufficientSpaceException() { }

        /// <summary>
        /// Initializes a new instance of <see cref="InsufficientSpaceException"/> with the specified error message
        /// </summary>
        /// <param name="message"></param>
        public InsufficientSpaceException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of <see cref="InsufficientSpaceException"/> with the specified error message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public InsufficientSpaceException(string message, Exception inner) : base(message, inner) { }
        
        /// <summary>
        /// Initializes a new instance of <see cref="InsufficientSpaceException"/> with the serialized data
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InsufficientSpaceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
