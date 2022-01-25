using System.Security.Cryptography;
using Authentication.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Services;

public class UsersService: IUsersService
{
    private readonly IServiceProvider _serviceProvider;

    public UsersService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<List<User>> GetUsers()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CiunexDbContext>();
        if (context.Users == null)
            return new List<User>();
        return await context.Users.ToListAsync();
    }

    public async Task<User?> CreateUser(User user)
    {
        user.Salt = GenerateSalt();
        user.PasswordHashed = HashPassword(user.Password, user.Salt);
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CiunexDbContext>();
        if (context.Users == null)
            return null;
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByUserName(string name)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CiunexDbContext>();
        if (context.Users == null)
            return null;
        return await context.Users.Where(u => u.Name == name).FirstOrDefaultAsync();
    }
    
    public async Task<User?> GetByEmail(string email)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CiunexDbContext>();
        if (context.Users == null)
            return null;
        return await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserById(Guid id)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CiunexDbContext>();
        if (context.Users == null)
            return null;
        return await context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> CheckLoginData(LoginData loginData)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CiunexDbContext>();
        if (context.Users == null)
            return false;
        var selectedUser = await GetByUserName(loginData.Name);
        if (selectedUser == null)
            return false;
        var hashedPassword = HashPassword(loginData.Password, selectedUser.Salt);
        return hashedPassword == selectedUser.PasswordHashed;
    }

    private byte[] GenerateSalt()
    {
        var salt = new byte[128 / 8];
        var rngCsp = RandomNumberGenerator.Create();
        rngCsp.GetNonZeroBytes(salt);
        return salt;
    }

    private string HashPassword(string password, byte[] salt)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
    }
    
    // private CiunexDbContext? GetDbContext()
    // {
    //     using var scope = _serviceProvider.CreateScope();
    //     var context = scope.ServiceProvider.GetRequiredService<CiunexDbContext>();
    //     if (context.Users == null)
    //         return null;
    // }
}