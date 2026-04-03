namespace SecureFileTransfer.Core.Models;

public sealed class HandshakeRequest
{
    public required string SenderId { get; init; }
    public required string Nonce { get; init; }
    public ulong SequenceNumber { get; init; }
    public long TimestampUnixMs { get; init; }
    public required string SignatureBase64 { get; init; }
}
