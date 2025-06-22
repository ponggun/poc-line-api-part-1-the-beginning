using System;

namespace PocLineAPI.Application;

public interface IErrorLogService
{
    /// <summary>
    /// Logs an unexpected error and returns a unique error code.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <returns>The generated error code.</returns>
    string LogUnexpectedError(Exception exception);
}
