using apbd_8_s22085.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace apbd_8_s22085.Database;

public class DatabaseContext : DbContext
{
    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<Doctor> Doctors { get; set; }
    public virtual DbSet<Medicament> Medicaments { get; set; }
    public virtual DbSet<Patient> Patients { get; set; }
    public virtual DbSet<Prescription> Prescription { get; set; }
    public virtual DbSet<PrescriptionMedicament> PrescriptionMedicament { get; set; }
    
}