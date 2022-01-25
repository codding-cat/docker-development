using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Models;

public class Token
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = "";
}