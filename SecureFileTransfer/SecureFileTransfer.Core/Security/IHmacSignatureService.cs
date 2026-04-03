namespace SecureFileTransfer.Core.Security;

public interface IHmacSignatureService
{
    string ComputeBase64(string canonicalPayload, byte[] key);
    bool VerifyBase64(string canonicalPayload, byte[] key, string signatureBase64);
}
