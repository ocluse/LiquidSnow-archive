using System;
using System.Net;
using System.Net.Http;

namespace Thismaker.Aba.Client.Core
{

    [Serializable]
    public class ClientException : Exception
    {
        public ClientException() { }
        public ClientException(string message) : base(message) { }
        public ClientException(Exception inner) : base(null, inner) { }
        public ClientException(string message, Exception inner) : base(message, inner) { }
        protected ClientException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class VersionMismatchException : ClientException
    {
        public VersionMismatchException() { }
        public VersionMismatchException(string message) : base(message) { }
        public VersionMismatchException(Exception inner) : base(inner) { }
        public VersionMismatchException(string message, Exception inner) : base(message, inner) { }
        protected VersionMismatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class ConnectionFailedException : ClientException
    {
        public ConnectionFailedException() { }
        public ConnectionFailedException(string message) : base(message) { }
        public ConnectionFailedException(Exception inner) : base(inner) { }
        public ConnectionFailedException(string message, Exception inner) : base(message, inner) { }
        protected ConnectionFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class ExpiredTokenException : ClientException
    {
        public ExpiredTokenException() { }
        public ExpiredTokenException(string message) : base(message) { }
        public ExpiredTokenException(string message, Exception inner) : base(message, inner) { }
        protected ExpiredTokenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class SimpleRequestException : HttpRequestException
    {
        public HttpStatusCode StatusCode => ResponseMessage.StatusCode;
        public HttpResponseMessage ResponseMessage { get; set; }
        public SimpleRequestException(HttpResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage;
        }
    }
}
