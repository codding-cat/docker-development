using Authentication.Models;

namespace Authentication.Interfaces;

public interface IUsersService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<(bool success, string accessToken, string refreshToken)> LoginAsync(LoginData data);
    Task<User> CreateAsync(User user);
}