using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Authentication.Models;

public class User
{
    public Guid Id { get; set; }
    
    [Required]
    [MinLength(4)]
    [MaxLength(32)]
    public string Name { get; set; } = "";
    
    [Required]
    [EmailAddress]
    [MaxLength(128)]
    public string Email { get; set; } = "";
    
    [Required]
    [MinLength(4)]
    [MaxLength(32)]
    [NotMapped]
    public string Password { get; set; } = "";
    
    public bool IsActive { get; set; }
    
    [Required]
    public Roles Role { get; set; }

    [JsonIgnore] public byte[] Salt { get; set; } = Array.Empty<byte>();
    [JsonIgnore] public string PasswordHashed { get; set; } = "";
    
}