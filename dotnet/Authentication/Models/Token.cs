using System.ComponentModel.DataAnnotations;

namespace Authentication.Models;

public class Token
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = "";
}