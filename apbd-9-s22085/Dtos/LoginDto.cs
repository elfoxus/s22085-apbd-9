using System.ComponentModel.DataAnnotations;

namespace apbd_8_s22085.Dtos;

public record LoginDto(
    [Required]
    [MaxLength(100)]
    string Login,
    [Required]
    [MaxLength(100)]
    string Password);