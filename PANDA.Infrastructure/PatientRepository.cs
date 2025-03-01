using Dapper;
using PANDA.Common;
using PANDA.Infrastructure.Core;
using PANDA.Infrastructure.Engines;
using PANDA.Infrastructure.Models;

namespace PANDA.Infrastructure;

public interface IPatientRepository : IRepository<PatientDao, string>
{
    IEnumerable<AppointmentDao> GetAppointments(string? patientId);
}

public class PatientRepository: DatabaseProvider, IPatientRepository
{
    public IEnumerable<PatientDao> List()
    {
        return Database.Query<PatientDao>("SELECT * FROM [Patient]");
    }

    public PatientDao? Get(string id)
    {
        return Database.QueryFirst<PatientDao>("SELECT * FROM [Patient] WHERE [PA_NhsNumber] = @Id", new { Id = id });
    }

    public OperationResult Add(PatientDao? entity, out PatientDao? addedEntity)
    {
        addedEntity = Database.QueryFirst<PatientDao>(@"
            INSERT INTO [Patient] (PA_NhsNumber, PA_Name, PA_Postcode, PA_DOB)
            OUTPUT INSERTED.*
            VALUES (@NhsNumber, @Name, @Postcode, @DOB);
        ", new
        {
            NhsNumber = entity.PA_NhsNumber,
            Name = entity.PA_Name,
            Postcode = entity.PA_Postcode,
            DOB = entity.PA_DOB
        });
        
        return addedEntity != null;
    }

    public OperationResult Update(PatientDao? entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return Database.Execute(@"
            UPDATE [Patient]
            SET
                [PA_Name] = @name,
                [PA_Postcode] = @postcode,
                [PA_DOB] = @dob
            WHERE
                [PA_NhsNumber] = @nhsNumber
        ", new
        {
            nhsNumber = entity.PA_NhsNumber,
            name = entity.PA_Name,
            postcode = entity.PA_Postcode,
            dob = entity.PA_DOB
        }) > 0;
    }

    public OperationResult Delete(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        return Database.Execute("DELETE FROM [Patient] WHERE [PA_NhsNumber] = @id", new { id }) > 0;
    }

    public IEnumerable<AppointmentDao> GetAppointments(string? patientId)
    {
        ArgumentNullException.ThrowIfNull(patientId, nameof(patientId));
        return Database.Query<AppointmentDao>("SELECT * FROM [Appointment] WHERE [PA_PatientId] = @patientId",
            new { patientId });
    }
}