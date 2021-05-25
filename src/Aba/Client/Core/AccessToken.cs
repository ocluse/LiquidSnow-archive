
using System;

namespace Thismaker.Aba.Client.Core
{
    public class AccessToken
    {
        private string value, key;
        private AccessTokenKind kind;
        private DateTimeOffset exipresOn;
        /// <summary>
        /// The actual value of the access-token, usually a random base64-encoded string
        /// </summary>
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// The key used to identify the <see cref="AccessToken"/> in the header for the HTTP requests
        /// </summary>
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// The kind of the <see cref="AccessToken"/>, usually Bearer, Basic or Custom for custom auth schemes
        /// </summary>
        public AccessTokenKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        /// <summary>
        /// The expiry time of the token. May be applicable to Bearer and custom tokens
        /// </summary>
        public DateTimeOffset ExpiresOn
        {
            get { return exipresOn; }
            set { exipresOn = value; }
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

        public static AccessToken Basic(string value)
        {
            var result = new AccessToken
            {
                Key = "Basic",
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
