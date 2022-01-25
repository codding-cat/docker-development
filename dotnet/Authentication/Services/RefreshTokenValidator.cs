using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Services;

public class RefreshTokenValidator : IRefreshTokenValidator
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<RefreshTokenValidator> _logger;

    public RefreshTokenValidator(JwtSettings jwtSettings, ILogger<RefreshTokenValidator> logger)
    {
        _jwtSettings = jwtSettings;
        _logger = logger;
    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RefreshTokenSecret)),
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            ClockSkew = TimeSpan.Zero
        };

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        try
        {
            jwtSecurityTokenHandler.ValidateToken(refreshToken, validationParameters,
                out SecurityToken validatedToken);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Jwt token validation error occurred");
        }
        return false;
    }

    public IEnumerable<Claim> GetClaimsFromExpiredToken(string expiredToken)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.AccessTokenSecret)),
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            ClockSkew = TimeSpan.Zero
        };
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        try
        {
            jwtSecurityTokenHandler.ValidateToken(expiredToken, validationParameters,
                out SecurityToken validatedToken);
            var tk = jwtSecurityTokenHandler.ReadJwtToken(expiredToken);
            
            return tk.Claims;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Jwt token validation error occurred");
        }
        return Enumerable.Empty<Claim>();
    }
}