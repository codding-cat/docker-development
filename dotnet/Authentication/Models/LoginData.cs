using System.ComponentModel.DataAnnotations;

namespace Authentication.Models;

public class LoginData
{
    [Required] public string Name { get; set; } = "";
    [Required] public string Password { get; set; } = "";
}