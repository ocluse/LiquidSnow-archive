using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client
{
    public class InitException : Exception
    {
        public InitExceptionKind Kind { get; set; }

        public InitException(string msg, InitExceptionKind kind) : base(msg)
        {
            Kind = kind;
        }
    }

    public enum InitExceptionKind
    {
        VersionMismatch,
        LoginFailure,
        HubException,
        NoEntitlements,
        RegisterFailure,
        ConnectionException,
    }
}
