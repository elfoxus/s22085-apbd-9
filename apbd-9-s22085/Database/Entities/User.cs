using System.ComponentModel.DataAnnotations;

namespace apbd_8_s22085.Database.Entities;

public class User
{
    [Key]
    public int IdUser { get; set; }
    [Required]
    [MaxLength(100)]
    public string Login { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = null!;
    [Required]
    public string Salt { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExp { get; set; }
}