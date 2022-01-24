using AuthorizationService.Models;

namespace AuthorizationService.Intefaces;

public interface IUserService
{
    bool Login(LoginModel user);
}