
using System;

namespace Thismaker.Aba.Client.Core
{
    public class AccessToken
    {
        private string value, key;
        private AccessTokenKind kind;
        private DateTimeOffset? exipresOn;
        private double expiryCushion = 5;
        private bool isAuthorizationHeader;

        /// <summary>
        /// The actual value of the access-token, usually a random base64-encoded string
        /// </summary>
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public bool IsAuthorizationHeader
        {
            get => isAuthorizationHeader;
            set => isAuthorizationHeader = value;
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
        public DateTimeOffset? ExpiresOn
        {
            get { return exipresOn; }
            set { exipresOn = value; }
        }

        /// <summary>
        /// The cushion time, in minutes that is added to the actual token expiry when checking whether the token is expired.
        /// The default value is 5 minutes
        /// </summary>
        public double ExpiryCushion
        {
            get => expiryCushion;
            set => expiryCushion = value;
        }

        /// <summary>
        /// Returns true if the access token has an expiry date that has been elapsed.
        /// For security, the method adds a margin of 5 minutes to the actual expiry time so that the access token
        /// is renewed well before expiry. This can be adjusted using 
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return (ExpiresOn.HasValue && DateTime.UtcNow > ExpiresOn.Value.AddMinutes(-ExpiryCushion).UtcDateTime);
        }

        /// <summary>
        /// Creates a Bearer access token
        /// </summary>
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

        /// <summary>
        /// Creates a basic access token
        /// </summary>
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

        /// <summary>
        /// Creates a custom access token
        /// </summary>
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
