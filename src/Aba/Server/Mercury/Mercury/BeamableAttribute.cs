using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Server.Mercury
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class BeamableAttribute : Attribute
    {
        private readonly List<string> _scopes;

        public string MethodName
        {
            get;set;
        }

        public bool Authorized
        {
            get; private set;
        }

        public BeamableAttribute()
        {
            Authorized = false;
            _scopes = new List<string>();
        }

        public BeamableAttribute(bool authorized)
        {
            Authorized = authorized;
            _scopes = new List<string>();
        }

        public BeamableAttribute(string[] scopes)
        {
            Authorized = true;
            _scopes = new List<string>(scopes);
        }

        public List<string> Scopes
        {
            get => _scopes;
        }
    }
}
