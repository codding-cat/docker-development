using System.Security.Authentication;
using System.Security.Cryptography;
using Authentication.Exceptions;
using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Authentication.Services;

public class UsersService: IUsersService
{
    private readonly IUsersProvider _provider;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    
    public UsersService(IUsersProvider provider,
        IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _provider = provider;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
    }
    
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _provider.GetAllAsync();
    }

    public async Task<(bool success, string accessToken, string refreshToken)> LoginAsync(LoginData data)
    {
        var user = await _provider.GetByNameAsync(data.Name);
        if (user == null)
            throw new AuthenticationException($"User with name {data.Name} not fount");
        var hashedPassword = HashPassword(data.Password, user.Salt);
        if (hashedPassword != user.PasswordHashed)
            throw new AuthenticationException($"Wrong password");
        var accessToken = _accessTokenService.Generate(user);
        var refreshToken = _refreshTokenService.Generate(user);
        return (true, accessToken, refreshToken);
    }

    public async Task<User> CreateAsync(User user)
    {
        var existingUser = await _provider.GetByNameAsync(user.Name);
        if (existingUser != null)
            throw new DataConflictException($"User with name '{user.Name}' already exist");
        existingUser = await _provider.GetByEmailAsync(user.Email);
        if (existingUser != null)
            throw new DataConflictException($"User with e-mail '{user.Email}' already exist");
        user.Salt = GenerateSalt();
        user.PasswordHashed = HashPassword(user.Password, user.Salt);
        var createdUser = await _provider.CreateAsync(user);
        if (createdUser == null)
            throw new Exception("Error occurred when creating a new user");
        return createdUser;
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
}