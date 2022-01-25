using System.Security.Claims;

namespace Authentication.Interfaces;

/// <summary>
/// Interface for validating refresh token.
/// </summary>
public interface IRefreshTokenValidator
{
    /// <summary>
    /// Validates refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>True if token is valid,otherwise false.</returns>
    bool ValidateRefreshToken(string refreshToken);

    IEnumerable<Claim> GetClaimsFromExpiredToken(string expiredToken);
}