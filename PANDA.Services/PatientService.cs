using PANDA.Common;
using PANDA.Common.Resources;
using PANDA.Infrastructure;
using PANDA.Services.Models;
using PANDA.Services.Core;

namespace PANDA.Services;

public interface IAppointmentService : IService<Appointment, string>
{}

public class AppointmentService(IAppointmentRepository appointmentRepository) : IAppointmentService
{
    public List<Appointment> List()
    {
        return appointmentRepository
            .List()
            .Select(Appointment.FromDatabase)
            .OfType<Appointment>()
            .ToList();
    }

    public Appointment? Get(string? id)
    {
        return !string.IsNullOrEmpty(id)
            ? Appointment.FromDatabase(appointmentRepository.Get(id))
            : null;
    }

    public OperationResult Add(Appointment? entity, out Appointment? addedEntity)
    {
        addedEntity = null;
        
        if (entity == null)
        {
            return Localisation.Error_NoEntity;
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