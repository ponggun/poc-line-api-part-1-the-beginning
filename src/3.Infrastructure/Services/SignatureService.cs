using System.Security.Cryptography;
using System.Text;
using PocLineAPI.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace PocLineAPI.Infrastructure.Services;

public class SignatureService : ISignatureService
{
    private readonly ILogger<SignatureService> _logger;
    public SignatureService(ILogger<SignatureService> logger)
    {
        _logger = logger;
    }

    public bool VerifySignature(string channelSecret, string requestBody, string? signature)
    {
        if (string.IsNullOrEmpty(signature)) return false;
        var key = Encoding.UTF8.GetBytes(channelSecret);
        var bodyBytes = Encoding.UTF8.GetBytes(requestBody);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(bodyBytes);
        var computedSignature = Convert.ToBase64String(hash);
        _logger.LogInformation("Computed Signature: {ComputedSignature}", computedSignature ?? string.Empty);
        return computedSignature == signature;
    }

    public string GenerateSignature(string secret, string body)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(bodyBytes);
        return Convert.ToBase64String(hash);
    }
}
