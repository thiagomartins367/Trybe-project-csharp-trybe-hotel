using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TrybeHotel.Dto;

namespace TrybeHotel.Services
{
    public class TokenGenerator
    {
        private readonly TokenOptions _tokenOptions;

        public TokenGenerator()
        {
            _tokenOptions = new TokenOptions();
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