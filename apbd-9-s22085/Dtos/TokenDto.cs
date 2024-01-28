using System.ComponentModel.DataAnnotations;

namespace apbd_8_s22085.Dtos;

public record TokenDto([Required]string token, [Required]string refreshToken);