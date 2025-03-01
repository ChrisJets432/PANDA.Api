
using PANDA.Infrastructure.Core;
using PANDA.Infrastructure.Models;
using PANDA.Services.Core;

namespace PANDA.Services.Models;

public class Appointment : IDaoConverter<AppointmentDao, Appointment>, IIdentifiable<string>
{
    public static class Statuses
    {
        public const string Attended = "attended";
        public const string Active = "active";
        public const string Cancelled = "cancelled";
        public const string Missed = "missed";
    }
    
    public string Id
    {
        get => Identifier.ToString();
    }
    
    private Guid? _identifier;
    public Guid Identifier
    {
        get => _identifier ?? Guid.NewGuid(); 
        set => _identifier = value;
    }
    public string PatientId { get; set; }
    public string Status { get; set; }
    public DateTimeOffset Time { get; set; }
    public TimeSpan Duration { get; set; }
    public string Department { get; set; }
    public string Clinician { get; set; }
    public string Postcode { get; set; }

    public static AppointmentDao? ToDatabase(Appointment? entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new AppointmentDao
        {
            AP_Identifier = entity.Identifier,
            AP_PatientId = entity.PatientId,
            AP_Status = entity.Status,
            AP_Time = entity.Time,
            AP_Duration = entity.Duration.Milliseconds,
            AP_Department = entity.Department,
            AP_Clinician = entity.Clinician,
            AP_Postcode = entity.Postcode
        };
    }

    public static Appointment? FromDatabase(AppointmentDao? dao)
    {
        if (dao == null)
        {
            return null;
        }

        return new Appointment
        {
            Identifier = dao.AP_Identifier,
            PatientId = dao.AP_PatientId,
            Status = dao.AP_Status,
            Time = dao.AP_Time,
            Duration = TimeSpan.FromMilliseconds(dao.AP_Duration),
            Department = dao.AP_Department,
            Clinician = dao.AP_Clinician,
            Postcode = dao.AP_Postcode
        };
    }
}