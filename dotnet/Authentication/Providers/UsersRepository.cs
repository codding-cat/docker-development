using System.Security.Cryptography;
using Authentication.Infrastructure;
using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Providers;

public class UsersRepository: IUsersRepository
{
    private readonly CiunexDbContext _dbContext;

    public UsersRepository(CiunexDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<User>> GetAllAsync()
    {
        if (_dbContext.Users == null)
            return new List<User>();
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> CreateAsync(User user)
    {
        if (_dbContext.Users == null)
            return null;
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByNameAsync(string name)
    {
        if (_dbContext.Users == null)
            return null;
        return await _dbContext.Users.Where(u => u.Name == name).FirstOrDefaultAsync();
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        if (_dbContext.Users == null)
            return null;
        return await _dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        if (_dbContext.Users == null)
            return null;
        return await _dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
    }
}