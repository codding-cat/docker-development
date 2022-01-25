using Authentication.Models;

namespace Authentication.Interfaces;

public interface IUsersService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<(bool success, string accessToken, string refreshToken)> LoginAsync(LoginData data, string ip);
    Task<User> CreateAsync(User user);

    Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string expiredToken, string userIp);
}