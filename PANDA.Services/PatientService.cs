using PANDA.Common;
using PANDA.Common.Extensions;
using PANDA.Common.Resources;
using PANDA.Infrastructure;
using PANDA.Services.Models;
using PANDA.Services.Core;

namespace PANDA.Services;

public interface IPatientService : IService<Patient, string>
{
    Patient? GetWithAppointments(string? id);
}

public class PatientService(IPatientRepository patientRepository) : IPatientService
{
    public List<Patient> List()
    {
        return patientRepository
            .List()
            .Select(Patient.FromDatabase)
            .OfType<Patient>()
            .ToList();
    }

    public Patient? Get(string? id)
    {
        return !string.IsNullOrEmpty(id)
            ? Patient.FromDatabase(patientRepository.Get(id))
            : null;
    }

    public Patient? GetWithAppointments(string? id)
    {
        ArgumentNullException.ThrowIfNull(id);
        
        var patient = Get(id) ?? throw new KeyNotFoundException("Patient not found");
        patient.Appointments = patientRepository
            .GetAppointments(patient.NhsNumber)
            .Select(Appointment.FromDatabase)
            .OfType<Appointment>()
            .ToList();
        
        return patient;
    }

    public OperationResult Add(Patient? entity, out Patient? addedEntity)
    {
        addedEntity = null;
        
        if (entity == null)
        {
            return Localisation.Error_NoEntity;
        }

        if (NhsExtensions.TryParse(entity.NhsNumber, out var nhsNumber) && !string.IsNullOrEmpty(nhsNumber))
        {
            entity.NhsNumber = nhsNumber;
        }
        else
        {
            return Localisation.Error_InvalidNhsNumber;
        }

        if (PostcodeExtensions.TryParse(entity.Postcode, out var postcode) && !string.IsNullOrEmpty(postcode))
        {
            entity.Postcode = postcode;
        }
        
        patientRepository.Add(Patient.ToDatabase(entity), out var added);

        if (added == null)
        {
            return Localisation.Error_EntityNotAdded;
        }
        
        addedEntity = Patient.FromDatabase(added);

        return true;
    }

    public OperationResult Update(Patient? entity)
    {
        return patientRepository.Update(Patient.ToDatabase(entity))
            ? true
            : Localisation.Error_NothingUpdated;
    }

    public OperationResult Delete(string? id)
    {
        return patientRepository.Delete(id)
            ? true
            : Localisation.Error_NothingDeleted;
    }
}