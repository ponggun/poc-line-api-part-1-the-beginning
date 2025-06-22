using System;
using Microsoft.Extensions.Logging;

namespace PocLineAPI.Application;

public class ErrorLogService : IErrorLogService
{
    private readonly ILogger<ErrorLogService> _logger;

    public ErrorLogService(ILogger<ErrorLogService> logger)
    {
        _logger = logger;
    }

    public string LogUnexpectedError(Exception exception)
    {
        var errorCode = Guid.NewGuid().ToString();
        _logger.LogError(exception, "Unexpected error occurred. Error Code: {ErrorCode}", errorCode);

        return errorCode;
    }
}

