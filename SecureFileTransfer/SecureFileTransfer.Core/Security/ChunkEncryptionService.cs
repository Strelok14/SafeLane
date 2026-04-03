using System.Security.Cryptography;

namespace SecureFileTransfer.Core.Security;

/// <summary>
/// AES-256-GCM authenticated encryption for file chunks.
/// Wire format: nonce(12 bytes) || ciphertext(n bytes) || tag(16 bytes) → base64.
/// </summary>
public sealed class ChunkEncryptionService : IChunkEncryptionService
{
    private const int NonceSize = 12; // 96-bit nonce, optimal for GCM
    private const int TagSize = 16;   // 128-bit authentication tag

    public string Encrypt(ReadOnlySpan<byte> plaintext, byte[] key)
    {
        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        // Serialize as nonce || ciphertext || tag
        var blob = new byte[NonceSize + ciphertext.Length + TagSize];
        nonce.CopyTo(blob, 0);
        ciphertext.CopyTo(blob, NonceSize);
        tag.CopyTo(blob, NonceSize + ciphertext.Length);

        return Convert.ToBase64String(blob);
    }

    public byte[] Decrypt(string encryptedBase64, byte[] key)
    {
        var blob = Convert.FromBase64String(encryptedBase64);

        if (blob.Length < NonceSize + TagSize)
        {
            throw new CryptographicException("Encrypted chunk data is too short.");
        }

        var nonce = blob.AsSpan(0, NonceSize);
        var tag = blob.AsSpan(blob.Length - TagSize, TagSize);
        var ciphertext = blob.AsSpan(NonceSize, blob.Length - NonceSize - TagSize);

        var plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return plaintext;
    }
}
