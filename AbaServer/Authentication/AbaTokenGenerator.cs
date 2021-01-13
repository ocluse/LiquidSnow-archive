using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Thismaker.Aba.Server.Authentication
{
    public class AbaTokenGenerator
    {
        private const string Secret = "TheWingadiumIsKay";

        public static string IssuerName = "";

        public static string GenerateToken(string username, string scopes, int expireMinutes = 20)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim("ApiScope", scopes)
            };

            var token = new JwtSecurityToken(IssuerName, IssuerName, 
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(20), 
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
