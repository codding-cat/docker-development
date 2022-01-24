using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthorizationService.Intefaces;
using AuthorizationService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthorizationService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController: ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    public TokenController(IConfiguration configuration,IUserService userService)
    {
        _configuration = configuration;
        _userService = userService;
    }
    
    [AllowAnonymous]
    [HttpPost(nameof(Login))]
    public IActionResult Login([FromBody] LoginModel data)
    {
        bool isValid = _userService.Login(data);
        if (isValid)
        {
            var tokenString = GenerateJwtToken(data.UserName);
            return Ok(new { Token = tokenString, Message = "Success" });
        }
        return BadRequest("Please pass the valid Username and Password");
    }
    
    [Authorize(AuthenticationSchemes = "OAuth")]
    [HttpGet(nameof(GetResult))]
    public IActionResult GetResult()
    {
        return Ok("API Validated");
    }

    private async Task<string> GenerateJwtToken(string userName)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        using var privateRsa = RSA.Create();
        var privateKeyPem =  await System.IO.File.ReadAllTextAsync(
            Path.Combine(path, _configuration["Jwt:privateKey"]));
        privateRsa.ImportFromPem(privateKeyPem);
        var privateKey = new RsaSecurityKey(privateRsa);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", userName) }),
            Expires = DateTime.UtcNow.AddHours(12),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                privateKey, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}