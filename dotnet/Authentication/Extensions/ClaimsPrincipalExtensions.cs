using System.Security.Claims;

namespace Authentication.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserLastIp(this ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);
        return claim?.Value;
    }
    
    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "id");
        return claim?.Value;
    }
}