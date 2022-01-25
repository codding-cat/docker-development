using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UsersController: ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await _usersService.GetUsers();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        var existingUser = await _usersService.GetByUserName(user.Name);
        if (existingUser != null)
            return BadRequest($"User with name '{user.Name}' already exist");
        existingUser = await _usersService.GetByEmail(user.Email);
        if (existingUser != null)
            return BadRequest($"User with e-mail '{user.Email}' already exist");
        var createdUser = await _usersService.CreateUser(user);
        if (createdUser == null)
            return BadRequest("Provided data error");
        return Ok(createdUser);
    }

    [HttpPost(nameof(Login))]
    public async Task<IActionResult> Login([FromBody] LoginData loginData)
    {
        var isLoggedIn = await _usersService.CheckLoginData(loginData);
        return Ok(isLoggedIn);
    }
}