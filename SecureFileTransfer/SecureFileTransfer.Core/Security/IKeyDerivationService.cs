namespace SecureFileTransfer.Core.Security;

public interface IKeyDerivationService
{
    byte[] DeriveKeyBytes(string sharedSecret);
}
