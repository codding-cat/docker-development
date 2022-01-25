using Authentication.Models;

namespace Authentication.Interfaces;

public interface IUsersRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> CreateAsync(User user);

    Task<User?> GetByNameAsync(string name);
    
    Task<User?> GetByEmailAsync(string email);
    
    Task<User?> GetByIdAsync(Guid id);

}