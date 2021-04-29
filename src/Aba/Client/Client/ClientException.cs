using System;

namespace Thismaker.Aba.Client
{
    public class ClientException : Exception
    {
        public ExceptionKind Kind { get; set; }

        public ClientException(string msg, ExceptionKind kind) : base(msg)
        {
            Kind = kind;
        }

        public ClientException(string msg, ExceptionKind kind, Exception innerException):base(msg, innerException) 
        {
            Kind = kind;
        }
    }

    public enum ExceptionKind
    {
        VersionMismatch,
        LoginFailure,
        HubException,
        NoEntitlements,
        RegisterFailure,
        ConnectionException,
        RequestFailed,
        AuthException,
        GenericException,

    }
}
