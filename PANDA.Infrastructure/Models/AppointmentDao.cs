namespace PANDA.Infrastructure.Models;

public class AppointmentDao
{
    public Guid AP_Identifier { get; set; }
    public string AP_PatientId { get; set; }
    public string AP_Status { get; set; }
    public DateTimeOffset AP_Time { get; set; }
    public double AP_Duration { get; set; }
    public string AP_Department { get; set; }
    public string AP_Clinician { get; set; }
    public string AP_Postcode { get; set; }
}