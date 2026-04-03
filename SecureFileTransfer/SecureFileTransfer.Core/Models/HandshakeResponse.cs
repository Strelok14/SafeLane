namespace SecureFileTransfer.Core.Models;

public sealed class HandshakeResponse
{
    public bool Success { get; init; }
    public required string Message { get; init; }
}
