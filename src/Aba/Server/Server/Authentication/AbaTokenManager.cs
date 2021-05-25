using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Thismaker.Aba.Server.Authentication
{
    public class AbaTokenManager
    {
        public AbaTokenManager(IConfiguration configuration)
        {
            configuration.Bind("AbaServer", this);
        }

        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }

        public string GenerateToken(string userId, List<Claim> claims, int expireMinutes = 20)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            if (claims == null)
            {
                claims = new List<Claim>();
            }

            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer=JwtIssuer, 
                IssuedAt=now, 
                Expires = expireMinutes == 0 ? null : now.AddMinutes(Convert.ToInt32(expireMinutes)),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }


        public ClaimsPrincipal GetPrincipal(string token,  bool requireExpTime=true, bool validateLifetime = true)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken) return null;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = requireExpTime,
                    ValidIssuer=JwtIssuer,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    IssuerSigningKey = securityKey,
                    ValidateLifetime=validateLifetime
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
