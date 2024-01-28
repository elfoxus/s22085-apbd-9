using System.ComponentModel.DataAnnotations;

namespace apbd_8_s22085.Database.Entities;

public class Medicament
{
    [Key]
    public int IdMedicament { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string Type { get; set; } = null!;
    
    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; } = null!;
}