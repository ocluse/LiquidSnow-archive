using System;
using System.Net;
using System.Net.Http;

namespace Thismaker.Aba.Client
{
    /// <summary>
    /// An exception that is thrown when an attempt is made to use an expired token
    /// </summary>
    [Serializable]
    public class ExpiredTokenException : Exception
    {
        /// <summary>
        /// Creates a new instance of the exception
        /// </summary>
        public ExpiredTokenException() { }
        
    }

    /// <summary>
    /// An exception that is thrown when the simple HTTP methods of the <see cref="ClientBase{TClient}"/> fail.
    /// </summary>
    /// <remarks>
    /// This exception will only be thrown if a valid HttpResponse message was received but it's processing failed, for example due to an incorrect HTTP status code.
    /// </remarks>
    [Serializable]
    public class SimpleRequestException : HttpRequestException
    {
        /// <summary>
        /// The Status Code of the response message.
        /// </summary>
        public HttpStatusCode StatusCode => ResponseMessage.StatusCode;
        
        /// <summary>
        /// The response message received from the server, whose processing failed
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; set; }
        
        /// <summary>
        /// Creates a new instance of the exception with the specidied response message
        /// </summary>
        public SimpleRequestException(HttpResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage;
        }
    }
}
