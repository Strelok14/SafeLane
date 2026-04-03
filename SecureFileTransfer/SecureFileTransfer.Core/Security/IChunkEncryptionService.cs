namespace SecureFileTransfer.Core.Security;

public interface IChunkEncryptionService
{
    /// <summary>
    /// Encrypts plaintext bytes with AES-256-GCM.
    /// Returns base64 blob: nonce(12) || ciphertext(n) || tag(16).
    /// </summary>
    string Encrypt(ReadOnlySpan<byte> plaintext, byte[] key);

    /// <summary>
    /// Decrypts a base64 blob produced by <see cref="Encrypt"/>.
    /// Throws <see cref="System.Security.Cryptography.CryptographicException"/> on tampering.
    /// </summary>
    byte[] Decrypt(string encryptedBase64, byte[] key);
}
