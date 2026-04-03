using System.Security.Cryptography;

namespace SecureFileTransfer.App.Infrastructure;

/// <summary>
/// Stores the shared secret encrypted with Windows DPAPI (current-user scope).
/// The secret is unreadable by other OS users or on other machines.
/// Falls back to the plain value from AppSettings when no protected file exists
/// (first launch / migration), then immediately re-saves it encrypted.
/// </summary>
public sealed class SecretStore
{
    private readonly string _secretFilePath;

    public SecretStore(string appDataRoot)
    {
        Directory.CreateDirectory(appDataRoot);
        _secretFilePath = Path.Combine(appDataRoot, "secret.dpapi");
    }

    /// <summary>
    /// Saves <paramref name="plainSecret"/> encrypted with DPAPI.
    /// </summary>
    public void Save(string plainSecret)
    {
        var plainBytes = System.Text.Encoding.UTF8.GetBytes(plainSecret);
        var encrypted = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(_secretFilePath, encrypted);
    }

    /// <summary>
    /// Loads and decrypts the shared secret.
    /// Returns <c>null</c> if the protected file does not exist yet (first launch).
    /// </summary>
    public string? TryLoad()
    {
        if (!File.Exists(_secretFilePath))
        {
            return null;
        }

        var encrypted = File.ReadAllBytes(_secretFilePath);
        var plainBytes = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
        return System.Text.Encoding.UTF8.GetString(plainBytes);
    }
}
