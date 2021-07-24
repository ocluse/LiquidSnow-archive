using System;
using System.Collections.Generic;

namespace Thismaker.Aba.Server.Mercury
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class BeamableAttribute : Attribute
    {
        public string MethodName
        {
            get; set;
        }

        public bool Authorized
        {
            get; private set;
        }

        public BeamableAttribute()
        {
            Authorized = false;
            Scopes = new List<string>();
        }

        public BeamableAttribute(bool authorized)
        {
            Authorized = authorized;
            Scopes = new List<string>();
        }

        public BeamableAttribute(string[] scopes)
        {
            Authorized = true;
            Scopes = new List<string>(scopes);
        }

        public List<string> Scopes { get; }
    }
}