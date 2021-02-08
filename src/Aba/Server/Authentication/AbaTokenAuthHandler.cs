using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Thismaker.Aba.Server.Authentication
{
    public class AbaTokenOptions : JwtBearerOptions
    {
    }

    public class AbaTokenAuthHandler : AuthenticationHandler<AbaTokenOptions>
    {

        private static List<string> hubPatterns;

        public static void AddHubPattern(string pattern)
        {
            if(hubPatterns==null) hubPatterns = new List<string>();
            hubPatterns.Add(pattern);
        }

        public static void RemoveHubPattern(string pattern)
        {
            if (hubPatterns == null) return;
            hubPatterns.Remove(pattern);
        }

        private static bool ContainsHubPattern(PathString path)
        {
            if (hubPatterns == null) return false;

            foreach(var pattern in hubPatterns)
            {
                if (path.StartsWithSegments(pattern)) return true;
            }
            return false;
        }

        private readonly AbaTokenManager _manager;
        private List<string> _requiredClaims;
        private readonly IAbaTokenKeeper _keeper;

        public AbaTokenAuthHandler(AbaTokenManager manager, IAbaTokenKeeper keeper, IOptionsMonitor<AbaTokenOptions> options, 
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) 
        {
            _manager = manager;
            _keeper = keeper;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var request = Context.Request;
            string authorization = request.Headers[HeaderNames.Authorization];

            string token = null;
            //Check for hub:

            bool isHub = false;
            var accessToken = Context.Request.Query["access_token"];
            var path = Context.Request.Path;
            if(!string.IsNullOrEmpty(accessToken) && (ContainsHubPattern(path)))
            {
                token = accessToken;
                isHub = true;
            }


            if (string.IsNullOrEmpty(authorization) && !isHub)
            {
                return AuthenticateResult.NoResult();
            }

            if(!isHub && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = authorization["Bearer ".Length..].Trim();
            }

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.NoResult();
            }

            //Check if requiredClaims is empty
            if (_requiredClaims == null) _requiredClaims = _keeper.GetRequiredClaims();

            //Handle Authenticate
            var principal = await AuthenticateJwtToken(token);

            if (principal == null)
            {
                return AuthenticateResult.Fail("Invalid token for AbaAuthenticateHandler");
            }
            else
            {
                return AuthenticateResult.Success(new AuthenticationTicket(principal, "Aba"));
            }
        }

        private bool ValidateToken(string token, out List<Claim> claims)
        {
            claims = new List<Claim>();
            
            var simplePrinciple= _manager.GetPrincipal(token);
            
            var identity = simplePrinciple?.Identity as ClaimsIdentity;

            if (identity == null) return false;

            if (!identity.IsAuthenticated) return false;

            //obtain the userId:
            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;


            if (string.IsNullOrEmpty(userId))
                return false;
            claims.Add(userIdClaim);

            //Check if user exists in the system:
            if (!_keeper.CheckIfTokenExists(userId, token)) return false;

            //Add the required claims, returning false if a required claim is not found
            foreach (var claimName in _requiredClaims)
            {
                var userClaim = identity.FindFirst(claimName);
                if (userClaim==null||string.IsNullOrEmpty(userClaim.Value)) return false;

                claims.Add(userClaim);
            }

            return true;
        }

        protected Task<ClaimsPrincipal> AuthenticateJwtToken(string token)
        {
            if(ValidateToken(token, out var claims))
            {

                var identity = new ClaimsIdentity(claims, "Aba");
                var user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<ClaimsPrincipal>(null);
        }
    }
}

