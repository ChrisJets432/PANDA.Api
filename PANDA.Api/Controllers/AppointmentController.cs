using Microsoft.AspNetCore.Mvc;
using PANDA.Api.Models;
using PANDA.Common;
using PANDA.Common.Extensions;
using PANDA.Common.Resources;
using PANDA.Services;
using PANDA.Services.Models;

namespace PANDA.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientController(
    ILogger<PatientController> logger,
    IPatientService patientService
) : BaseController<IPatientService, PatientBody, Patient, string>(logger, patientService)
{ }