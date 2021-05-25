using System;
using System.Collections.Generic;

namespace Thismaker.Aba.Server.Mercury
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class RequireTokenValidationAttribute : Attribute
    {
        private readonly List<string> _scopes;

        public RequireTokenValidationAttribute()
        {
            _scopes = new List<string>();
        }


        public RequireTokenValidationAttribute(string[] scopes)
        {
            _scopes = new List<string>(scopes);
        }

        public List<string> Scopes
        {
            get => _scopes;
        }
    }
}
