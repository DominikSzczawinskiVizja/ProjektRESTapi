using api.Models;
using api.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api.Services.AuthS
{
    public class TokenService(IOptions<JwtOptions> jwtOptions) : ITokenService //injectuje jwt.asp.net 
    {
        private readonly JwtOptions _jwt = jwtOptions.Value; //wyciaga obiekt z ustawieniami _jwt.key/Issuer/Audience/ExpiresMinutes

        public string CreateAccessToken(User user)
        {
            var claims = new List<Claim> //claims = dane o user ktore sa zapisywane w tokenie. Po weryfikacji tokena beda dostepne jako httpcontext.user
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()), // user.Id
                new(ClaimTypes.Email, user.Email), // user email
                new(ClaimTypes.Role, user.Role) //user role do weryfikacji uprawnienien itp.
            };
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)); //tworzy z secret klucz kryptograficzny
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256); //algorytm do podpisu

            var token = new JwtSecurityToken( //tworzy token
            issuer: _jwt.Issuer, //Issuer 
            audience: _jwt.Audience, // Audience
            claims: claims, //id email role
            notBefore: DateTime.UtcNow, //nie przed teraz
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiresMinutes), //wygasa teraz + expires minutes
            signingCredentials: creds //podpis 
                );

            return new JwtSecurityTokenHandler().WriteToken(token); //zmienia obiekt token na string ktory zwraca dla klienta
        }
    }

}
