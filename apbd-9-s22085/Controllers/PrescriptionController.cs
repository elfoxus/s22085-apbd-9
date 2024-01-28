using apbd_8_s22085.Database;
using apbd_8_s22085.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apbd_8_s22085.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionController : ControllerBase
{
    private readonly ILogger<PrescriptionController> _logger;
    private readonly DatabaseContext _database;

    public PrescriptionController(ILogger<PrescriptionController> logger, DatabaseContext database)
    {
        _logger = logger;
        _database = database;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPrescriptionFor(int prescriptionId)
    {
        var prescription = await _database.Prescription
            .Where(p => p.IdPrescription == prescriptionId)
            .Include(p => p.Doctor)
            .Include(p => p.Patient)
            .Include(p => p.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .Select(p => new PrescriptionInfoDto(
                p.IdPrescription,
                p.Date,
                p.DueDate,
                new DoctorInfoDto(p.Doctor.IdDoctor, p.Doctor.FirstName, p.Doctor.LastName, p.Doctor.Email),
                new PatientInfoDto(p.Patient.IdPatient, p.Patient.FirstName, p.Patient.LastName, p.Patient.Birthdate),
                p.PrescriptionMedicaments.Select(pm => new MedicamentInfoDto(pm.Medicament.IdMedicament,
                    pm.Medicament.Name,
                    pm.Medicament.Description,
                    pm.Medicament.Type,
                    pm.Dose ?? 0,
                    pm.Details))
            ))
            .FirstOrDefaultAsync();

        if (prescription == null)
        {
            return NotFound();
        }

        return Ok(prescription);
    }
}