using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Services;

public class TokenGenerator: ITokenGenerator
{
    public string Generate(string secretKey, string issuer, string audience, 
        int expires, IEnumerable<Claim>? claims = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken securityToken = new(issuer,audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(expires),
            credentials);
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}