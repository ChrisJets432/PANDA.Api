using Newtonsoft.Json;
using PANDA.Api.Core;
using PANDA.Common;
using PANDA.Common.Extensions;
using PANDA.Common.Resources;
using PANDA.Services.Models;

namespace PANDA.Api.Models;

public class AppointmentBody : IRequestConverter<AppointmentBody, Appointment, string>
{
    [JsonIgnore]
    public string? Id
    {
        get => Identifier;
    }
    
    [JsonProperty("id")]
    public string Identifier { get; set; }
    
    [JsonProperty("patient")]
    public string? PatientId { get; set; }
    
    [JsonProperty("status")]
    public string? Status { get; set; }
    
    [JsonProperty("time")]
    public string? Time { get; set; }
    
    [JsonProperty("duration")]
    public string? Duration { get; set; }
    
    [JsonProperty("department")]
    public string? Department { get; set; }
    
    [JsonProperty("postcode")]
    public string? Postcode { get; set; }

    public static AppointmentBody? ToRequestEntity(Appointment? entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new AppointmentBody
        {
            Identifier = entity.Identifier.ToString(),
            PatientId = entity.PatientId,
            Status = entity.Status,
            Time = entity.Time.ToString("yyyy-MM-ddTHH:mm:sszzz"),
            Duration = TimeSpanExtensions.StringDuration(entity.Duration),
            Department = entity.Department,
            Postcode = entity.Postcode
        };
    }

    public static OperationResult FromRequestEntity(AppointmentBody? request, out Appointment? appointment)
    {
        appointment = null;
        
        if (request == null)
        {
            return Localisation.Error_NoEntity;
        }

        if (string.IsNullOrEmpty(request.PatientId))
        {
            return Localisation.Error_InvalidField.FormatWith(Localisation.Field_PatientId);
        }

        if (!NhsExtensions.TryParse(request.PatientId, out var patientId))
        {
            return Localisation.Error_InvalidField.FormatWith(Localisation.Field_PatientId);
        }

        if (string.IsNullOrEmpty(request.Time))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_Time);
        }

        if (!DateTimeOffset.TryParse(request.Time, out var time))
        {
            return Localisation.Error_InvalidField.FormatWith(Localisation.Field_Time);
        }

        if (string.IsNullOrEmpty(request.Duration))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_Duration);
        }

        if (!TimeSpanExtensions.TryParse(request.Duration, out var duration) && !duration.HasValue)
        {
            return Localisation.Error_InvalidField.FormatWith(Localisation.Field_Duration);
        }

        if (string.IsNullOrEmpty(request.Postcode))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_Postcode);
        }

        if (!PostcodeExtensions.TryParse(request.Postcode, out var postcode))
        {
            return Localisation.Error_InvalidField.FormatWith(Localisation.Field_Postcode);
        }

        if (string.IsNullOrEmpty(request.Status))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_Status);
        }

        if (string.IsNullOrEmpty(request.Department))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_Department);
        }

        var identifier = Guid.NewGuid();

        if (!string.IsNullOrEmpty(request.Identifier) && !Guid.TryParse(request.Identifier, out identifier))
        {
            return Localisation.Error_InvalidField.FormatWith(Localisation.Field_Identifier);
        }

        appointment = new Appointment
        {
            Identifier = identifier,
            PatientId = patientId,
            Status = request.Status,
            Time = time,
            Duration = duration.Value,
            Department = request.Department,
            Postcode = postcode
        };

        return true;
    }
}