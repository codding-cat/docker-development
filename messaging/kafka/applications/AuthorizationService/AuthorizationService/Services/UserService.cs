using AuthorizationService.Intefaces;
using AuthorizationService.Models;

namespace AuthorizationService.Services;

public class UserService: IUserService
{
    public bool Login(LoginModel user)
    {
        return true; //(user.UserName.Equals("Adm") && user.Password.Equals("Adm"));
    }
}