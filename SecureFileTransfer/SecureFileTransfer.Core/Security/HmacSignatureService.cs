using System.Security.Cryptography;
using System.Text;

namespace SecureFileTransfer.Core.Security;

public sealed class HmacSignatureService : IHmacSignatureService
{
    public string ComputeBase64(string canonicalPayload, byte[] key)
    {
        var payloadBytes = Encoding.UTF8.GetBytes(canonicalPayload);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(payloadBytes);
        return Convert.ToBase64String(hash);
    }

    public bool VerifyBase64(string canonicalPayload, byte[] key, string signatureBase64)
    {
        try
        {
            var expected = Convert.FromBase64String(ComputeBase64(canonicalPayload, key));
            var actual = Convert.FromBase64String(signatureBase64);
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }
        catch
        {
            return false;
        }
    }
}
