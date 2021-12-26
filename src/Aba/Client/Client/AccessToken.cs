
using System;

namespace Thismaker.Aba.Client
{
    /// <summary>
    /// A class simplyfing dealing with access tokens, and checking their validity.
    /// </summary>
    public class AccessToken
    {

        /// <summary>
        /// The value added to the request header
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
        /// </summary>
        /// <remarks>
        /// A cushion is added meaning that the token is reported as expired at least a while before the actual expiration time.
        /// This is important in cases where the server and client times may not be in perfect sync.
        /// This margin is specified by the <see cref="ExpiryCushion"/> value.
        /// </remarks>
        /// <returns>True if the token has expired and needs to be refreshed</returns>
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
