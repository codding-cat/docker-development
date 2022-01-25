using Authentication.Models;

namespace Authentication.Interfaces;

public interface IUsersService
{
    Task<List<User>> GetUsers();
    Task<User?> CreateUser(User user);

    Task<User?> GetByUserName(string name);
    
    Task<User?> GetByEmail(string name);
    
    Task<User?> GetUserById(Guid id);

    Task<bool> CheckLoginData(LoginData loginData);
}