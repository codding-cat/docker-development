namespace Authentication.Models;

public class JwtSettings
{
    public string AccessTokenSecret { get; set; } = "";
    public string RefreshTokenSecret { get; set; } = "";
    public int AccessTokenExpirationSeconds { get; set; }
    public int RefreshTokenExpirationSeconds { get; set; }
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
}