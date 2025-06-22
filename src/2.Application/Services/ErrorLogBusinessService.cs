using Microsoft.Extensions.Logging;

namespace PocLineAPI.Application;

public interface IErrorLogBusinessService
{
    /// <summary>
    /// Logs an unexpected error and returns a unique error code.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <returns>The generated error code.</returns>
    string LogUnexpectedError(Exception exception);
}

public class ErrorLogBusinessService : IErrorLogBusinessService
{
    private readonly ILogger<ErrorLogBusinessService> _logger;

    public ErrorLogBusinessService(ILogger<ErrorLogBusinessService> logger)
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

