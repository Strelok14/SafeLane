namespace SecureFileTransfer.Core.Models;

public sealed class TrustedNodeConfig
{
    public required string NodeId { get; set; }
    public required string DisplayName { get; set; }
    public required string BaseUrl { get; set; }
    public string? SharedSecretOverride { get; set; }
}
