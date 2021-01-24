using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client
{
    public class ClientException : Exception
    {
        public ExceptionKind Kind { get; set; }

        public ClientException(string msg, ExceptionKind kind) : base(msg)
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
    }
}
