using System.Security.Authentication;
using System.Security.Cryptography;
using Authentication.Exceptions;
using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Authentication.Services;

/// <summary>
/// Users service
/// </summary>
public class UsersService: IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly ITokensRepository _tokensRepository;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    
    public UsersService(IUsersRepository usersRepository,
        ITokensRepository tokensRepository,
        IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _usersRepository = usersRepository;
        _tokensRepository = tokensRepository;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
    }
    
    /// <summary>
    /// Get all users async
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _usersRepository.GetAllAsync();
    }

    /// <summary>
    /// Login async
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="AuthenticationException"></exception>
    public async Task<(bool success, string accessToken, string refreshToken)> LoginAsync(LoginData data)
    {
        var user = await _usersRepository.GetByNameAsync(data.Name);
        if (user == null)
            throw new AuthenticationException($"User with name {data.Name} not fount");
        var hashedPassword = HashPassword(data.Password, user.Salt);
        if (hashedPassword != user.PasswordHashed)
            throw new AuthenticationException($"Wrong password");
        
        var accessToken = _accessTokenService.Generate(user);
        var refreshToken = _refreshTokenService.Generate(user);
        
        await _tokensRepository.AddToken(new Token
        {
            UserId = user.Id,
            RefreshToken = refreshToken
        });
        
        return (true, accessToken, refreshToken);
    }

    /// <summary>
    /// Create user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="DataConflictException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<User> CreateAsync(User user)
    {
        var existingUser = await _usersRepository.GetByNameAsync(user.Name);
        if (existingUser != null)
            throw new DataConflictException($"User with name '{user.Name}' already exist");
        existingUser = await _usersRepository.GetByEmailAsync(user.Email);
        if (existingUser != null)
            throw new DataConflictException($"User with e-mail '{user.Email}' already exist");
        user.Salt = GenerateSalt();
        user.PasswordHashed = HashPassword(user.Password, user.Salt);
        var createdUser = await _usersRepository.CreateAsync(user);
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