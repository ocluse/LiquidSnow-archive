using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Thismaker.Aba.Server.Authentication;

namespace Microsoft.AspNetCore.Builder
{
    public static class AuthExtensions
    {
        public static AuthenticationBuilder AddThismakerAba(this AuthenticationBuilder builder)
        {
            builder.AddScheme<AbaTokenOptions, AbaTokenAuthHandler>(JwtBearerDefaults.AuthenticationScheme, options=>{ });
            return builder;
        }
    }
}

