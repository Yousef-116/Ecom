using Ecom.Core.Entities;
using Ecom.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories.Service
{
    public class GenerateToken : IGenerateToken
    {
        private readonly IConfiguration Configuration;
        public GenerateToken(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public string GetAndGenerateToken(AppUser user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email)
            };
            SecurityTokenDescriptor token = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(2),
                Issuer = Configuration["Token:Issure"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Token:Secret"])), SecurityAlgorithms.HmacSha256Signature),
                NotBefore = DateTime.UtcNow
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(token);
            return handler.WriteToken(securityToken);

        }
    }
}
