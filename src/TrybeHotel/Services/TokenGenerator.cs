using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Services
{
    public class TokenGenerator
    {
        private readonly TokenOptions _tokenOptions;

        public TokenGenerator()
        {
            var envSecretKey = Environment.GetEnvironmentVariable("AUTH_TOKEN_SECRET_KEY");
            var envExpiresDay = Environment.GetEnvironmentVariable("AUTH_TOKEN_EXPIRE_DAYS");
            var expiresDayIsNumber = int.TryParse(envExpiresDay, out int expiresDay);
            _tokenOptions = new TokenOptions
            {
                Secret = envSecretKey is not null ? envSecretKey : "",
                ExpiresDay = expiresDayIsNumber ? expiresDay : 7,
            };
        }

        public string Generate(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = AddClaims(user),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.Secret)),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Expires = DateTime.Now.AddDays(_tokenOptions.ExpiresDay)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static ClaimsIdentity AddClaims(UserDto user)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claims.AddClaim(new Claim(ClaimTypes.Role, user.UserType));
            return claims;
        }
    }
}
