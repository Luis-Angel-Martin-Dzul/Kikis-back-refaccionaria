using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kikis_back_refaccionaria.Infrastructure.Utilities {

    public class JWToken {


        public static string Generator(IConfiguration _configuration, UserAuthRES account) {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", account.Id.ToString()),
                new Claim("firtsname", account.FirstName),
                new Claim("lastname", account.LastName),
                new Claim("curp", account.Curp),
                new Claim("email", account.Email),
                new Claim("rol", account.Rol.ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: credentials
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
