using Microsoft.Extensions.Options;
using PocLineAPI.Application.Models;
using PocLineAPI.Application.Interfaces;

namespace PocLineAPI.Application.Services
{
    public class SoftwareBusinessService : ISoftwareBusinessService
    {
        private readonly SoftwareOptions _softwareOptions;
        public SoftwareBusinessService(IOptions<SoftwareOptions> softwareOptions)
        {
            _softwareOptions = softwareOptions.Value;
        }
        public string GetVersion()
        {
            return _softwareOptions.Version;
        }
    }
}
