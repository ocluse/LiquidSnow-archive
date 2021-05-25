using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Thismaker.Aba.Server.Mercury.Hosted
{
    [Authorize]
    [Route(Route)]
    public class AuthenticatorController : ControllerBase
    {
        public const string Route = "thismaker/aba/mercury/server/authenticator";

        [HttpGet]
        public ClaimsPrincipal Get()
        {
            return User;
        }
    }
}
