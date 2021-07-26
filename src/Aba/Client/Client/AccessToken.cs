
using System;

namespace Thismaker.Aba.Client
{
    public class AccessToken
    {

        /// <summary>
        /// The actual value of the access-token, usually a base64-encoded string
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The key added to the request headers, e.g Authorization
        /// </summary>
        public string HeaderName { get; set; }

        /// <summary>
        /// The scheme of the access token, e.g Bearer
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// The expiry time of the token, after which it should be renewed
        /// </summary>
        public DateTimeOffset? ExpiresOn { get; set; }

        /// <summary>
        /// The cushion time, in minutes that is added to the actual token expiry when checking whether the token is expired.
        /// The default value is 5 minutes
        /// </summary>
        public double ExpiryCushion { get; set; } = 5;

        /// <summary>
        /// Returns true if the access token has an expiry date that has been elapsed.
        /// For security, the method adds a margin of 5 minutes to the actual expiry time so that the access token
        /// is renewed well before expiry. This can be adjusted using 
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return ExpiresOn.HasValue && DateTime.UtcNow > ExpiresOn.Value.AddMinutes(-ExpiryCushion).UtcDateTime;
        }

        /// <summary>
        /// Creates a Bearer access token
        /// </summary>
        public static AccessToken Bearer(string value, DateTimeOffset? expiry = null)
        {
            AccessToken result = new AccessToken
            {
                HeaderName = "Authorization",
                Scheme = "Bearer",
                Value = value,
                ExpiresOn = expiry
            };
            return result;
        }

        /// <summary>
        /// Creates a basic access token
        /// </summary>
        public static AccessToken Basic(string value, DateTimeOffset? expiry = null)
        {
            AccessToken result = new AccessToken
            {
                HeaderName = "Authorization",
                Scheme = "Basic",
                Value = value,
                ExpiresOn = expiry
            };

            return result;
        }
    }
}
