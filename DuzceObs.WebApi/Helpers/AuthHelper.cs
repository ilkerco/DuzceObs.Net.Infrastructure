using DuzceObs.Core.Model.Entities;
using DuzceObs.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Helpers
{
    public class AuthHelper : IAuthHelper
    {
        private readonly UserManager<User> _userManager;
        public AuthHelper(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> GenerateJwtToken(User user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id),
                    new Claim(ClaimTypes.Name,user.Tc),
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super secret ilker key"));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(365),
                    SigningCredentials = credentials,

                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
