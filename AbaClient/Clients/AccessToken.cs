using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Aba.Client.Clients
{
    public class AccessToken
    {
        private string value, key;
        private AccessTokenKind kind;

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public AccessTokenKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        public static AccessToken Bearer(string value)
        {
            var result = new AccessToken
            {
                Key = "Bearer",
                Kind = AccessTokenKind.Bearer,
                Value = value
            };
            return result;
        }

        public static AccessToken Custom(string key, string value)
        {
            var result = new AccessToken
            {
                Key = key,
                Kind = AccessTokenKind.Custom,
                Value = value
            };
            return result;
        }
    }

    public enum AccessTokenKind
    {
        Bearer,
        Basic,
        Custom,
    }
}
