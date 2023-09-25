using MessagePublisher.WebApi.Properties;
using MessagePublisher.WebApi.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MessagePublisher.WebApi.Mappers;

public static class ErrorResponseMapper
{
    public static ErrorResponse FromModelState(ModelStateDictionary modelState)
    {
        var errors = modelState.Values.SelectMany(m => m.Errors);
        return new ErrorResponse
        {
            Code = 400,
            Title = "Bad Request",
            Message = Resources.Bad_Request_Message,
            Details = errors.Select(e => e.ErrorMessage).ToArray()
        };
    }
    
    public static ErrorResponse? FromException(Exception? ex)
    {
        if (ex == null) return null;
        
        return new ErrorResponse
        {
            Code = ex.HResult,
            Title = "Internal Server Error",
            Message = ex.Message,
            InnerError = FromException(ex.InnerException),
            TraceId = ex.StackTrace
        };
    }
}