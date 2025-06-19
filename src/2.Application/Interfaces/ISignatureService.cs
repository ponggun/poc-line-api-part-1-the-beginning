namespace PocLineAPI.Application.Interfaces;

public interface ISignatureService
{
    bool VerifySignature(string channelSecret, string requestBody, string? signature);
    string GenerateSignature(string secret, string body);
}
