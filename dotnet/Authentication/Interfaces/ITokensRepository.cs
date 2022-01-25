using Authentication.Models;

namespace Authentication.Interfaces;

public interface ITokensRepository
{
    Task AddToken(Token token);
    Task RemoveTokensByUserId(Guid userId);
    Task UpdateToken(Token tokenToRemove, Token newToken);
    Task<Token?> GetTokenByUserId(Guid userId);
}