using PANDA.Infrastructure.Core;
using PANDA.Infrastructure.Models;
using PANDA.Services.Core;

namespace PANDA.Services.Models;

public class Patient : IDaoConverter<PatientDao, Patient>, IIdentifiable<string>
{
    public string Id
    {
        get => NhsNumber;
    }
    
    public string NhsNumber { get; set; }
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Postcode { get; set; }
    public List<Appointment> Appointments { get; set; } = [];

    public static PatientDao? ToDatabase(Patient? entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new PatientDao
        {
            PA_NhsNumber = entity.NhsNumber,
            PA_Name = entity.Name,
            PA_DOB = entity.DateOfBirth.ToOADate(),
            PA_Postcode = entity.Postcode
        };
    }

    public static Patient? FromDatabase(PatientDao? dao)
    {
        if (dao == null)
        {
            return null;
        }

        return new Patient
        {
            NhsNumber = dao.PA_NhsNumber,
            Name = dao.PA_Name,
            DateOfBirth = DateTime.FromOADate(dao.PA_DOB),
            Postcode = dao.PA_Postcode
        };
    }
}