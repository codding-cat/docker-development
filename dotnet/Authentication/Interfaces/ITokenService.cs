using Authentication.Models;

namespace Authentication.Interfaces;

/// <summary>
/// Interface for generating token.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates token based on user information.
    /// </summary>
    /// <param name="user"><see cref="User"/> instance.</param>
    /// <returns>Generated token.</returns>
    string Generate(User user);
}

public interface IRefreshTokenService : ITokenService { }

public interface IAccessTokenService : ITokenService { }