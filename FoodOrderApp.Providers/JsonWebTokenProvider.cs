using FoodOrderApp.Interfaces.Providers;
using FoodOrderApp.Models.UserModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodOrderApp.Providers
{
    public class JsonWebTokenProvider : IJsonWebTokenProvider
    {
        private IConfiguration _configuration;

        public JsonWebTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            _configuration = null;
        }

        /*https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api*/
        public string GenerateJwtBearer(UserModel authenticatedUser, IList<string> roles)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("AuthKeys:DefaultKey").Value));
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, authenticatedUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, authenticatedUser.UserName),
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
            };

            if(roles != null && roles.Count > 0)
            {         
                // add users roles to token if they exist
                foreach(string role in roles)
                {
                    tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            string tokenToReturn = tokenHandler.WriteToken(token);

            return tokenToReturn;
        }
    }
}
