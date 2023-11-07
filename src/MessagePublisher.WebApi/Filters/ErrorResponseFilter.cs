﻿using MessagePublisher.WebApi.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessagePublisher.WebApi.Filters;

public class ErrorResponseFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var errorResponse = ErrorResponseMapper.FromException(context.Exception);
        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = ResolveStatusCode(context.Exception)
        };
    }

    private static int ResolveStatusCode(Exception exception)
    {
        return exception switch
        {
            KeyNotFoundException => 404,
            ArgumentException => 400,
            TimeoutException => 408,
            _ => 500
        };
    }
    
}