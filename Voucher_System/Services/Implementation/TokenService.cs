using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Voucher_System.Models;
using Voucher_System.Services.Interfaces;

namespace Voucher_System.Services.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(config["TokenKey"]));
        }
        public string creatToken(string userName)
        {
            var claims = new List<Claim>{

                //This will create the claim with its key ==> nameId and value ==>user name
                new Claim(JwtRegisteredClaimNames.NameId,userName)
            };

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // describe token properties
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentials
            };

            //used to handle the creation of the token and serlizing the token to JSON
            var tokenHandler = new JwtSecurityTokenHandler();

            //creating the token itself
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //WriteToken ==> serialize the jwt token
            return tokenHandler.WriteToken(token);
        }
    }
}
