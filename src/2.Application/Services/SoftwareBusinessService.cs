using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace PocLineAPI.Application;

public interface ISoftwareBusinessService
{
    string GetVersion();
}

public class SoftwareBusinessService : ISoftwareBusinessService
{
    private readonly SoftwareOptions _softwareOptions;
    private readonly ILogger<SoftwareBusinessService> _logger;
    public SoftwareBusinessService(IOptions<SoftwareOptions> softwareOptions, ILogger<SoftwareBusinessService> logger)
    {
        _softwareOptions = softwareOptions.Value;
        _logger = logger;
    }
    public string GetVersion()
    {
        if (string.IsNullOrEmpty(_softwareOptions.Version))
        {
            _logger.LogError("Software version is not configured.");
            throw new InvalidOperationException("Software version is not configured.");
        }

        _logger.LogInformation("Retrieved software version: {Version}", _softwareOptions.Version);
        return _softwareOptions.Version;
    }
}
