using Authentication.Infrastructure;
using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Providers;

public class TokensRepository: ITokensRepository
{
    private readonly CiunexDbContext _dbContext;

    public TokensRepository(CiunexDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddToken(Token token)
    {
        if (_dbContext.Tokens == null)
            return;
        await _dbContext.Tokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveTokensByUserId(Guid userId)
    {
        if (_dbContext.Tokens == null)
            return;
        var tokensToRemove = _dbContext.Tokens.Where(x => x.UserId == userId).AsEnumerable();
        _dbContext.Tokens.RemoveRange(tokensToRemove);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateToken(Token tokenToRemove, Token newToken)
    {
        if (_dbContext.Tokens == null)
            return;
        _dbContext.Tokens.Remove(tokenToRemove);
        await _dbContext.Tokens.AddAsync(newToken);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Token?> GetTokenByUserId(Guid userId)
    {
        if (_dbContext.Tokens == null)
            return null;
        return await _dbContext.Tokens.FirstOrDefaultAsync(
            x => x.UserId == userId);
    }
}