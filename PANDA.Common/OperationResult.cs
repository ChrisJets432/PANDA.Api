
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace PANDA.Common;

public class OperationResult
{
    public static readonly OperationResult Successful = new();
    public static readonly OperationResult Failed = new() { Success = false };
    
    public bool Success { get; set; }

    public int StatusCode { get; set; } = StatusCodes.Status200OK;
    
    public List<OperationResultMessage> Errors { get; set; } = [];
    
    public List<OperationResultMessage> Warnings { get; set; } = [];
    
    public List<OperationResultMessage> Infos { get; set; } = [];

    public OperationResult(bool ean = true)
    {
        Success = ean;
    }

    public OperationResult(string message) : this(false)
    {
        Errors.Add(message);
    }

    public OperationResult(params OperationResultMessage[] errors) : this(false)
    {
        Errors.AddRange(errors);
    }
    
    public void AddError(string message)
    {
        Success = false;
        Errors.Add(message);
    }

    public void AddError(OperationResultMessage message)
    {
        Success = false;
        Errors.Add(message);
    }

    public void AddWarning(string message)
    {
        Warnings.Add(message);
    }

    public void AddWarning(OperationResultMessage message)
    {
        Warnings.Add(message);
    }

    public void AddInfo(string message)
    {
        Infos.Add(message);
    }

    public void AddInfo(OperationResultMessage message)
    {
        Warnings.Add(message);
    }

    public void AddMessage(OperationResultMessage message)
    {
        Warnings.Add(message);
    }

    public void AddError(Exception ex)
    {
        Success = false;
        Errors.Add(ex);
    }
    
    public void Merge(OperationResult result)
    {
        if (!result.Success)
        {
            Success = false;
        }

        Errors.AddRange(result.Errors);
        Warnings.AddRange(result.Warnings);
        Infos.AddRange(result.Infos);
    }

    public static OperationResult operator +(OperationResult result, string message)
    {
        result.AddError(message);
        return result;
    }

    public static OperationResult operator +(OperationResult result, Exception exception)
    {
        result.AddError(exception);
        return result;
    }

    public static OperationResult operator +(OperationResult result, OperationResultMessage message)
    {
        result.AddError(message);
        return result;
    }

    public static OperationResult operator +(OperationResult a, OperationResult b)
    {
        a.Merge(b);
        return a;
    }
    
    public static implicit operator OperationResult(Exception ex)
    {
        return new OperationResult(new OperationResultMessage(ex.Message) { Exception = ex});
    }

    public static implicit operator OperationResult(string message)
    {
        return new OperationResult(message);
    }

    public static implicit operator bool(OperationResult result)
    {
        return result.Success;
    }

    public static implicit operator OperationResult(bool ean)
    {
        return ean ? Successful : Failed;
    }
}

public class OperationResult<T> : OperationResult
{
    public T? Data { get; set; }
}

public class OperationResultMessage
{
    public string Message { get; set; }
    
    [Newtonsoft.Json.JsonIgnore, JsonIgnore]
    public Exception? Exception { get; set; }

    public OperationResultMessage(string message)
    {
        Message = message;
    }
    
    public static implicit operator OperationResultMessage(string message)
    {
        return new OperationResultMessage(message);
    }

    public static implicit operator OperationResultMessage(Exception ex)
    {
        return new OperationResultMessage(ex.Message);
    }

    public override string ToString()
    {
        return Message;
    }
}