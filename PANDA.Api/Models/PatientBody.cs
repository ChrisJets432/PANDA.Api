using System.Globalization;
using Newtonsoft.Json;
using PANDA.Api.Core;
using PANDA.Common;
using PANDA.Common.Extensions;
using PANDA.Common.Resources;
using PANDA.Infrastructure.Core;
using PANDA.Infrastructure.Models;
using PANDA.Services.Models;

namespace PANDA.Api.Models;

public class PatientBody : IRequestConverter<PatientBody, Patient, string>
{
    [JsonIgnore]
    public string? Id
    {
        get => NhsNumber;
    }
    
    [JsonProperty("nhs_number")]
    public string? NhsNumber { get; set; }
    
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("date_of_birth")]
    public string? DateOfBirth { get; set; }
    
    [JsonProperty("postcode")]
    public string? Postcode { get; set; }

    [JsonProperty("appointments", NullValueHandling = NullValueHandling.Ignore)]
    public List<AppointmentBody>? Appointments { get; set; }

    public static PatientBody? ToRequestEntity(Patient? entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new PatientBody
        {
            NhsNumber = entity.NhsNumber,
            Name = entity.Name,
            DateOfBirth = entity.DateOfBirth.ToString("yyyy-MM-dd"),
            Postcode = entity.Postcode
        };
    }

    public static OperationResult FromRequestEntity(PatientBody? request, out Patient? patient)
    {
        patient = null;
        
        if (request == null)
        {
            return Localisation.Error_NoEntity;
        }

        if (string.IsNullOrEmpty(request.NhsNumber))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_NhsNumber);
        }

        if (!NhsExtensions.TryParse(request.NhsNumber, out var nhsNumber))
        {
            return Localisation.Error_InvalidField.FormatWith(Localisation.Field_NhsNumber);
        }

        if (string.IsNullOrEmpty(request.Name))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_Name);
        }

        if (string.IsNullOrEmpty(request.DateOfBirth))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_DateOfBirth);
        }

        if (!DateTime.TryParseExact(request.DateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var dateOfBirth))
        {
            return Localisation.Error_InvalidField.FormatWith(Localisation.Field_DateOfBirth);
        }

        if (string.IsNullOrEmpty(request.Postcode))
        {
            return Localisation.Error_FieldRequired.FormatWith(Localisation.Field_Postcode);
        }

        if (!PostcodeExtensions.TryParse(request.Postcode, out var postcode))
        {
            return Localisation.Error_InvalidField.FormatWith(Localisation.Field_Postcode);
        }

        patient = new Patient
        {
            NhsNumber = nhsNumber,
            Name = request.Name,
            DateOfBirth = dateOfBirth,
            Postcode = postcode,
        };

        return true;
    }
}