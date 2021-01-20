using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Aba.Client.Clients
{
    /// <summary>
    /// A base class that inherits from <see cref="ClientBase"/>.
    /// Useful when using AzureADB2C in it's default state. Note that for this client,
    /// <see cref="ClientBase.AccessToken"/> is predetemined and cannot be changed
    /// </summary>
    public abstract class MsalClient<T>:ClientBase<T> where T:MsalClient<T>
    {
        /// <summary>
        /// The identifier of the client, Provided in the Azure Portal.
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// The sign-up sign in policy to be used in token acquisiton
        /// </summary>
        public string PolicySUSI { get; set; }

        /// <summary>
        /// The application's redirect uri, as selected in the Azure Portal
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// The address of the Azure ADB2C Tenant
        /// </summary>
        public string Tenant { get; set; }

        public string AadInstance { get; set; }

        public string AuthorityBase
        {
            get
            {
                return $"https://{AadInstance}/tfp/{Tenant}/";
            }
        }

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

        public override void Configure(IConfiguration config)
        {
            base.Configure(config);

            var abaSection = config.GetSection("AbaClient");
            ClientID = abaSection.GetSection("ClientID").Value;
            AadInstance = abaSection.GetSection("AadInstance").Value;
            Tenant = abaSection.GetSection("Tenant").Value;
            PolicySUSI = abaSection.GetSection("PolicySUSI").Value;

            //Api Scopes
            ApiScopes = new List<string>();

            var scopes = abaSection.GetSection("Scopes").GetChildren();

            foreach(var scope in scopes)
            {
                ApiScopes.Add(scope.Value);
            }

            PublicClient = PublicClientApplicationBuilder.Create(ClientID)
                .WithB2CAuthority(AuthoritySUSI)
                .WithRedirectUri(RedirectUri)
                .Build();
        }

        public abstract Task Login();
        public abstract Task Logout();

    }
}
