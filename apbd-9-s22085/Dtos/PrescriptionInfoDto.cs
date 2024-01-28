namespace apbd_8_s22085.Dtos;

public record PrescriptionInfoDto(
    int IdPrescription,
    DateTime Date,
    DateTime DueDate,
    DoctorInfoDto Doctor,
    PatientInfoDto Patient,
    IEnumerable<MedicamentInfoDto> Medicaments);
    
public record PatientInfoDto(
    int IdPatient,
    string FirstName,
    string LastName,
    DateTime Birthdate);
    
public record MedicamentInfoDto(
    int IdMedicament,
    string Name,
    string Description,
    string Type,
    int Dose,
    string Details);