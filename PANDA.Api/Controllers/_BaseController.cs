using Microsoft.AspNetCore.Mvc;
using PANDA.Api.Core;
using PANDA.Common;
using PANDA.Common.Extensions;
using PANDA.Common.Resources;
using PANDA.Services.Core;

namespace PANDA.Api.Models;

public class BaseController<TService, TBody, TObject, TId>(
    ILogger<object> logger,
    TService service
) : ControllerBase
    where TService : IService<TObject, TId>
    where TBody : class, IRequestConverter<TBody, TObject, TId>
    where TObject : class, IIdentifiable<TId>
{
    private TService _service = service;

    [HttpGet("{id}")]
    public ActionResult<OperationResult<TBody>> Get(TId id)
    {
        var result = new OperationResult<TBody>();
        
        try
        {
            var patient = _service.Get(id);

            if (patient == null)
            {
                result.AddError(Localisation.Error_NoEntity);
            }
            else
            {
                result.Data = TBody.ToRequestEntity(patient);
            }
        }
        catch (KeyNotFoundException ex)
        {
            result.AddError(Localisation.Error_NoEntityType.FormatWith(ex.Message));
            result.StatusCode = StatusCodes.Status404NotFound;
        }
        catch (Exception ex)
        {
            result.AddError(Localisation.Error_ServerException);
            result.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogError(ex, ex.Message);
        }
        
        return result;
    }

    [HttpPost]
    public ActionResult<OperationResult<TBody>> Add(TBody request)
    {
        var result = new OperationResult<TBody>();

        try
        {
            var isValid = TBody.FromRequestEntity(request, out var patient);

            if (!isValid)
            {
                result.Merge(isValid);
                return result;
            }

            if (patient == null)
            {
                result.AddError(Localisation.Error_NoEntity);
                return result;
            }
            
            var serviceResult = _service.Add(patient, out var addedEntity);

            if (!serviceResult)
            {
                result.Merge(serviceResult);
                return result;
            }
            
            result.AddInfo(Localisation.Message_SuccessAdd);
            result.Data = TBody.ToRequestEntity(addedEntity);
        }
        catch (KeyNotFoundException ex)
        {
            result.AddError(Localisation.Error_NoEntityType.FormatWith(ex.Message));
            result.StatusCode = StatusCodes.Status404NotFound;
        }
        catch (Exception ex)
        {
            result.AddError(Localisation.Error_ServerException);
            result.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogError(ex, ex.Message);
        }

        return result;
    }

    [HttpPut("{id}")]
    public ActionResult<OperationResult<TBody>> Update(TId id, TBody request)
    {
        var result = new OperationResult<TBody>();

        try
        {
            if (!id.Equals(request.Id))
            {
                result.AddError(Localisation.Error_InvalidField.FormatWith(Localisation.Field_NhsNumber));
                result.StatusCode = StatusCodes.Status400BadRequest;
                return result;
            }

            var existingPatient = _service.Get(id);

            if (existingPatient == null)
            {
                result.AddError(Localisation.Error_EntityNotFound);
                result.StatusCode = StatusCodes.Status404NotFound;
                return result;
            }

            var isValid = TBody.FromRequestEntity(request, out var patient);

            if (!isValid)
            {
                result.Merge(isValid);
                return result;
            }

            var updateResult = _service.Update(patient);

            if (!updateResult)
            {
                result.Merge(updateResult);
                return result;
            }
        }
        catch (KeyNotFoundException ex)
        {
            result.AddError(Localisation.Error_NoEntityType.FormatWith(ex.Message));
            result.StatusCode = StatusCodes.Status404NotFound;
        }
        catch (Exception ex)
        {
            result.AddError(Localisation.Error_ServerException);
            result.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogError(ex, ex.Message);
        }
        
        return result;
    }
    
    [HttpDelete("{id}")]
    public ActionResult<OperationResult> Delete(TId id)
    {
        var result = new OperationResult<TBody>();

        try
        {
            var existingPatient = _service.Get(id);

            if (existingPatient == null)
            {
                result.AddError(Localisation.Error_EntityNotFound);
                result.StatusCode = StatusCodes.Status404NotFound;
                return result;
            }

            var updateResult = _service.Delete(existingPatient.Id);

            if (!updateResult)
            {
                result.Merge(updateResult);
                return result;
            }

            result.AddInfo(Localisation.Message_SuccessDelete);
        }
        catch (KeyNotFoundException ex)
        {
            result.AddError(Localisation.Error_NoEntityType.FormatWith(ex.Message));
            result.StatusCode = StatusCodes.Status404NotFound;
        }
        catch (Exception ex)
        {
            result.AddError(Localisation.Error_ServerException);
            result.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogError(ex, ex.Message);
        }
        
        return result;
    }
}