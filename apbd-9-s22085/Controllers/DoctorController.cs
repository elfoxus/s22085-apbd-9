using apbd_8_s22085.Database;
using apbd_8_s22085.Database.Entities;
using apbd_8_s22085.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apbd_8_s22085.Controllers;

[ApiController]
[Route("[controller]")]
public class DoctorController : ControllerBase
{

    private readonly ILogger<DoctorController> _logger;
    private readonly DatabaseContext _database;

    public DoctorController(ILogger<DoctorController> logger, DatabaseContext database)
    {
        _logger = logger;
        _database = database;
    }
    
    [HttpGet]
    public async Task<IEnumerable<DoctorInfoDto>> GetDoctors()
    {
        return await _database.Doctors
            .Select(d => new DoctorInfoDto(
                d.IdDoctor,
                d.FirstName,
                d.LastName,
                d.Email
            )).ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDoctor(int id)
    {
        var found = await _database.Doctors
            .Where(d => d.IdDoctor == id)
            .Select(d => new DoctorInfoDto(
                d.IdDoctor,
                d.FirstName,
                d.LastName,
                d.Email
            )).FirstOrDefaultAsync();

        if (found == null)
        {
            return NotFound();
        }

        return Ok(found);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddDoctor(NewDoctorDto dto)
    {
        var doctor = new Doctor
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };
        await _database.Doctors.AddAsync(doctor);
        await _database.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDoctor(int id, DoctorUpdateDto dto)
    {
        var doctor = await _database.Doctors.FindAsync(id);
        if (doctor == null)
        {
            return NotFound();
        }
        doctor.FirstName = dto.FirstName;
        doctor.LastName = dto.LastName;
        doctor.Email = dto.Email;
        await _database.SaveChangesAsync();
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var doctor = await _database.Doctors.FindAsync(id);
        if (doctor == null)
        {
            return NotFound();
        }
        _database.Doctors.Remove(doctor);
        await _database.SaveChangesAsync();
        return Ok();
    }
}