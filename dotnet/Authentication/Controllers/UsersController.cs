using System.Security.Authentication;
using Authentication.Exceptions;
using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
    [Authorize]
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
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginData loginData)
    {
        try
        {
            var userIp = HttpContext.Connection.RemoteIpAddress != null?
                HttpContext.Connection.RemoteIpAddress.ToString() : "";
            var (isLoggedIn, access, refresh) = await _usersService.LoginAsync(loginData, userIp);
            HttpContext.Response.Cookies.Append(
                "refresh-token",
                refresh,
                new CookieOptions
                {
                    HttpOnly = true
                });
            return Ok(access);
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

    [HttpPost(nameof(RefreshToken))]
    
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody]string expiredToken)
    {
        var userIp = HttpContext.Connection.RemoteIpAddress != null?
            HttpContext.Connection.RemoteIpAddress.ToString() : "";
        try
        {
            var (access, refresh) = await _usersService.RefreshTokenAsync(expiredToken, userIp);
            HttpContext.Response.Cookies.Append(
                "refresh-token",
                refresh,
                new CookieOptions
                {
                    HttpOnly = true
                });
            return Ok(access);
        }
        catch (NullReferenceException e)
        {
            Console.WriteLine(e);
            return ValidationProblem(e.Message);
        }
        catch (AuthenticationException e)
        {
            Console.WriteLine(e);
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Problem(e.Message);
        }
    }
}