using System.Security.Cryptography;
using System.Text;

namespace SecureFileTransfer.Core.Security;

public sealed class KeyDerivationService : IKeyDerivationService
{
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("SecureFileTransfer.Salt.v1");

    public byte[] DeriveKeyBytes(string sharedSecret)
    {
        var secret = string.IsNullOrWhiteSpace(sharedSecret) ? "default-demo-secret" : sharedSecret;
        return Rfc2898DeriveBytes.Pbkdf2(secret, Salt, 150_000, HashAlgorithmName.SHA256, 32);
    }
}
