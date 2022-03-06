using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thismaker.Aba.Client.SignalR;

namespace Thismaker.Aba.Client.Msal
{
    ///<summary>
    /// Contains utility methods for obtaining access tokens for users in Azure Active Directory and access a protected API.
    ///</summary>
    public abstract class MsalClientBase<T> : SignalRClientBase<T> where T : MsalClientBase<T>
    {
        #region Properties

        /// <summary>
        /// Gets the ID token obtained after logging a user in.
        /// </summary>
        public string IdToken { get; protected set; }

        /// <summary>
        /// Gets the refresh token obtained after logging in a user.
        /// </summary>
        public string RefreshToken { get; protected set; }

        ///<inheritdoc cref="ClientBase{TClient}.Context"/>
        public new IMsalContext Context => (IMsalContext)base.Context;

        /// <summary>
        /// The identifier of the client, Provided in the Azure Portal.
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// The sign-up,sign in policy to be used in token acquisiton.
        /// </summary>
        public string PolicySUSI { get; set; }

        /// <summary>
        /// The application's redirect uri, as provided in the Azure Portal.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// The address of the Azure ADB2C Tenant, e.g thismaker.onmicrosoft.com
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the AadIstance e.g thismaker.b2clogin.com
        /// </summary>
        public string AadInstance { get; set; }

        /// <summary>
        /// Derived from the <see cref="AadInstance"/> and <see cref="Tenant"/>
        /// </summary>
        public virtual string AuthorityBase => $"https://{AadInstance}/tfp/{Tenant}/";

        /// <summary>
        /// Derived from the <see cref="AuthorityBase"/> and the <see cref="PolicySUSI"/>
        /// </summary>
        public virtual string AuthoritySUSI => $"{AuthorityBase}{PolicySUSI}";

        /// <summary>
        /// The Api Scopes that the client wishes to access when acquirinng the <see cref=" AccessToken"/>
        /// </summary>
        public List<string> ApiScopes { get; set; }

        /// <summary>
        /// The Msal app, contains app information as well as a scheme for acquiring tokens
        /// </summary>
        public IPublicClientApplication PublicClient { get; protected set; }

        /// <summary>
        /// Gets the args used to obtain the access token.
        /// </summary>
        public TokenAccessArgs ApiTokenAccessArgs { get; protected set; }

        /// <summary>
        /// An event that is fired whenever the access token is renewed, silently or interactively.
        /// </summary>
        protected event Action<AuthenticationResult> TokenRenewed;

        #endregion

        #region Base Overrides
        /// <summary>
        /// Initializes the HttpClient, HubConnection and the Public client.
        /// Should be called only once during the lifetime of the application.
        /// Overriding this method allows you to customize how the app will be initialized
        /// </summary>
        public override void MakeApp()
        {
            base.MakeApp();

            PublicClient = PublicClientApplicationBuilder.Create(ClientID)
               .WithB2CAuthority(AuthoritySUSI)
               .WithRedirectUri(RedirectUri)
               .WithLogging(OnMsalLog, LogLevel.Error)
               .Build();
        }

        ///<inheritdoc/>
        public override void SetContext(IContext context)
        {
            if (context is IMsalContext)
            {
                base.SetContext(context);
            }
            else
            {
                throw new ArgumentException($"{nameof(Context)} must derive from {nameof(IMsalContext)}");
            }
        }

        #endregion

        #region Virtuals
        /// <summary>
        /// Override to capture any Msal logs
        /// </summary>
        protected virtual void OnMsalLog(LogLevel level, string message, bool containsPii)
        {

        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Should be preferrably called immediately after the the user has logged in.
        /// This is only useful if the built in token renewal method is used.
        /// </summary>
        /// <param name="account">The user account</param>
        /// <param name="scopes">The scopes requred by the application</param>
        protected void SetApiTokenAccessArgs(IAccount account, IEnumerable<string> scopes)
        {
            ApiTokenAccessArgs = new TokenAccessArgs
            {
                UserAccount = account,
                Scopes = new List<string>(scopes)
            };
        }

        /// <summary>
        /// Helper used to parse the ID token and obtain the various parts.
        /// </summary>
        /// <returns></returns>
        protected JsonDocument ParseIdToken()
        {
            // Parse the idToken to get user info
            string idPart = IdToken.Split('.')[1];
            idPart = Base64UrlDecode(idPart);
            return JsonDocument.Parse(idPart);
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + ((4 - (s.Length % 4)) % 4), '=');
            byte[] byteArray = Convert.FromBase64String(s);
            string decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

        ///<inheritdoc/>
        protected override async Task RenewAccessTokenAsync()
        {
            AuthenticationResult result;
            try
            {
                result = await PublicClient.AcquireTokenSilent(ApiTokenAccessArgs.Scopes, ApiTokenAccessArgs.UserAccount).ExecuteAsync();
                if (string.IsNullOrEmpty(result.AccessToken))
                {
                    throw new MsalUiRequiredException("404", "Access token was null");
                }
            }
            catch (MsalUiRequiredException)
            {
                result = await PublicClient.AcquireTokenInteractive(ApiTokenAccessArgs.Scopes)
                    .WithB2CAuthority(AuthoritySUSI)
                    .WithAccount(ApiTokenAccessArgs.UserAccount)
                    .WithParentActivityOrWindow(Context.GetMainWindow())
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
            AccessToken = AccessToken.Bearer(result.AccessToken, result.ExpiresOn);

            IdToken = result.IdToken;

            TokenRenewed?.Invoke(result);
        }
        #endregion
    }
}
