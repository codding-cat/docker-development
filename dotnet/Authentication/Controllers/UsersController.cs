using System.Security.Authentication;
using Authentication.Exceptions;
using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status200OK)]
public class UsersController: ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }
    
    /// <summary>
    /// Get all users
    /// </summary>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        try
        {
            return Ok(await _usersService.GetAllAsync());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem("Internal server error");
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="user"></param>
    /// <response code="400">Validation problem</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        try
        {
            return Ok(await _usersService.CreateAsync(user));
        }
        catch (DataConflictException e)
        {
            Console.WriteLine(e);
            return ValidationProblem(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="loginData"></param>
    /// <returns></returns>
    /// <response code="401">Unathorized</response>
    /// /// <response code="500">Internal server error</response>
    [HttpPost(nameof(Login))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginData loginData)
    {
        try
        {
            return Ok(await _usersService.LoginAsync(loginData));
        }
        catch (AuthenticationException e)
        {
            Console.WriteLine(e);
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem();
        }
    }
}