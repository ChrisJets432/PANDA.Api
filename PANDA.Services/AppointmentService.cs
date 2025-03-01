using PANDA.Common;
using PANDA.Common.Extensions;
using PANDA.Common.Resources;
using PANDA.Infrastructure;
using PANDA.Services.Models;
using PANDA.Services.Core;

namespace PANDA.Services;

public interface IAppointmentService : IService<Appointment, string>
{}

public class AppointmentService(IAppointmentRepository appointmentRepository, IPatientService patientService) : IAppointmentService
{
    public List<Appointment> List()
    {
        var list = appointmentRepository
            .List()
            .Select(Appointment.FromDatabase)
            .OfType<Appointment>()
            .ToList();

        foreach (var appointment in list.Where(x =>
                     x.Status == Appointment.Statuses.Active && x.Time <= DateTimeOffset.Now))
        {
            appointment.Status = Appointment.Statuses.Missed;
            Update(appointment);
        }
        
        return list;
    }

    public Appointment? Get(string? id)
    {
        var appointment = !string.IsNullOrEmpty(id)
            ? Appointment.FromDatabase(appointmentRepository.Get(id))
            : null;

        if (appointment != null
            && appointment.Status == Appointment.Statuses.Active && appointment.Time <= DateTimeOffset.Now)
        {
            appointment.Status = Appointment.Statuses.Missed;
            Update(appointment);
        }
        
        return appointment;
    }

    public OperationResult Add(Appointment? entity, out Appointment? addedEntity)
    {
        addedEntity = null;
        
        if (entity == null)
        {
            return Localisation.Error_NoEntity;
        }

        if (string.IsNullOrEmpty(entity.PatientId))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_PatientId);
        }
        else
        {
            var patient = patientService.Get(entity.PatientId) ?? throw new KeyNotFoundException(Localisation.Entity_Patient);
            
            // do patient work here
        }
        
        appointmentRepository.Add(Appointment.ToDatabase(entity), out var added);

        if (added == null)
        {
            return Localisation.Error_EntityNotAdded;
        }
        
        addedEntity = Appointment.FromDatabase(added);

        return true;
    }

    public OperationResult Update(Appointment? entity)
    {
        if (entity == null)
        {
            return Localisation.Error_NoEntity;
        }
        
        if (string.IsNullOrEmpty(entity.PatientId))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_PatientId);
        }
        else
        {
            var patient = patientService.Get(entity.PatientId) ?? throw new KeyNotFoundException(Localisation.Entity_Patient);
            
            // do patient work here
        }
        
        if (entity.Status == Appointment.Statuses.Active && entity.Time <= DateTimeOffset.Now)
        {
            entity.Status = Appointment.Statuses.Missed;
            Update(entity);
        }
        
        return appointmentRepository.Update(Appointment.ToDatabase(entity))
            ? true
            : Localisation.Error_NothingUpdated;
    }

    public OperationResult Delete(string? id)
    {
        return appointmentRepository.Delete(id)
            ? true
            : Localisation.Error_NothingDeleted;
    }
}