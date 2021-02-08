using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thismaker.Aba.Client.Msal
{
    /// <summary>
    /// A base class that inherits from <see cref="ClientBase"/>.
    /// Useful when using AzureADB2C in it's default state. Note that for this client,
    /// <see cref="ClientBase.AccessToken"/> is predetemined and cannot be changed
    /// </summary>
    public abstract class MsalClient<T>:ClientBase<T> where T:MsalClient<T>
    {
        #region Properties

        ///<inheritdoc/>
        public new IMsalContext Context { get; set; }

        /// <summary>
        /// The identifier of the client, Provided in the Azure Portal.
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// The sign-up,sign in policy to be used in token acquisiton
        /// </summary>
        public string PolicySUSI { get; set; }

        /// <summary>
        /// The application's redirect uri, as provided in the Azure Portal
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// The address of the Azure ADB2C Tenant, e.g thismaker.onmicrosoft.com
        /// </summary>
        public string Tenant { get; set; }

        public string AadInstance { get; set; }

        /// <summary>
        /// Derived from the <see cref="AadInstance"/> and <see cref="Tenant"/>
        /// </summary>
        public string AuthorityBase
        {
            get
            {
                return $"https://{AadInstance}/tfp/{Tenant}/";
            }
        }

        /// <summary>
        /// Derived from the <see cref="AuthorityBase"/> and the <see cref="PolicySUSI"/>
        /// </summary>
        public string AuthoritySUSI
        {
            get
            {
                return $"{AuthorityBase}{PolicySUSI}";
            }
        }

        /// <summary>
        /// The Api Scopes that the client wishes to access when acquirinng the <see cref=" AccessToken"/>
        /// </summary>
        public List<string> ApiScopes { get; set; }

        /// <summary>
        /// The Msal app, contains app information as well as a scheme for acquiring tokens
        /// </summary>
        public IPublicClientApplication PublicClient { get; protected set; }

        #endregion

        #region BaseOverrides
        /// <inheritdoc/>
        public override void MakeApp()
        {
            base.MakeApp();

            PublicClient = PublicClientApplicationBuilder.Create(ClientID)
               .WithB2CAuthority(AuthoritySUSI)
               .WithRedirectUri(RedirectUri)
               .Build();

        }
        #endregion

        #region Abstracts
        /// <summary>
        /// When overriden in a derived class, starts the relevant login process
        /// </summary>
        /// <returns></returns>
        public abstract Task Login();

        /// <summary>
        /// When overriden in a derived class, starts the relevant logout process
        /// </summary>
        /// <returns></returns>
        public abstract Task Logout();
        #endregion

    }
}
