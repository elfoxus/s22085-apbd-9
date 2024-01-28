using System.ComponentModel.DataAnnotations;

namespace apbd_8_s22085.Dtos;

public record DoctorUpdateDto(
    [Required] string FirstName,
    [Required] string LastName,
    [Required] [EmailAddress] string Email);