using Newtonsoft.Json;
using PANDA.Infrastructure.Core;
using PANDA.Infrastructure.Models;
using PANDA.Services.Models;

namespace PANDA.Api.Models;

public class Patient
{
    [JsonProperty("nhs_number")]
    public string NhsNumber { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("date_of_birth")]
    public string DateOfBirth { get; set; }
    
    [JsonProperty("postcode")]
    public string Postcode { get; set; }

    [JsonProperty("appointments", NullValueHandling = NullValueHandling.Ignore)]
    public List<Appointment>? Appointments { get; set; }
}