using Dapper;
using PANDA.Common;
using PANDA.Infrastructure.Core;
using PANDA.Infrastructure.Engines;
using PANDA.Infrastructure.Models;

namespace PANDA.Infrastructure;

public interface IAppointmentRepository : IRepository<AppointmentDao, string>
{
    IEnumerable<AppointmentDao> ByPatient(string? nhsNumber);
}

public class AppointmentRepository : DatabaseProvider, IAppointmentRepository
{
    public IEnumerable<AppointmentDao> List()
    {
        return Database.Query<AppointmentDao>("SELECT * FROM [Appointment]");
    }

    public AppointmentDao? Get(string? id)
    {
        return Database.QueryFirst<AppointmentDao>("SELECT * FROM [Appointment] where [AP_Identifier] = @id", new { id });
    }

    public OperationResult Add(AppointmentDao? entity, out AppointmentDao? addedEntity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        addedEntity = Database.QueryFirst<AppointmentDao>(@"
            INSERT INTO [Appointment] (AP_Identifier, AP_Postcode, AP_Status, AP_Department, AP_Clinician, AP_Duration, AP_Time, PA_PatientId)
            OUTPUT INSERTED.*
            VALUES (@identifier, @postcode, @status, @department, @clinician, @duration, @time, @patientId);
        ", new
        {
           identifier = entity.AP_Identifier,
           postcode = entity.AP_Postcode,
           status = entity.AP_Status,
           department = entity.AP_Department,
           clinician = entity.AP_Clinician,
           duration = entity.AP_Duration,
           time = entity.AP_Time,
           patientId = entity.AP_PatientId
        });
        
        return addedEntity != null;
    }

    public OperationResult Update(AppointmentDao? entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return Database.Execute(@"
            UPDATE [Appointment]
            SET
                [PA_PatientId] = @patientId,
                [AP_Time] = @time,
                [AP_Duration] = @duration,
                [AP_Department] = @department,
                [AP_Clinician] = @clinician,
                [AP_Status] = @status,
                [AP_Postcode] = @postcode
            WHERE
                [AP_Identifier] = @identifier
        ", new
        {
            identifier = entity.AP_Identifier,
            patientId = entity.AP_PatientId,
            time = entity.AP_Time,
            duration = entity.AP_Duration,
            department = entity.AP_Department,
            clinician = entity.AP_Clinician,
            status = entity.AP_Status,
            postcode = entity.AP_Postcode
        }) > 0;
    }

    public OperationResult Delete(string? id)
    {
        ArgumentNullException.ThrowIfNull(id);
        return Database.Execute("DELETE FROM [Appointment] WHERE [AP_Identifier] = @id", new { id }) > 0;
    }

    public IEnumerable<AppointmentDao> ByPatient(string? nhsNumber)
    {
        ArgumentNullException.ThrowIfNull(nhsNumber);
        return Database
            .Query<AppointmentDao>("SELECT * FROM [Appointment] WHERE PA_PatientId = @PatientId",
            new { PatientId = nhsNumber });
    }
}