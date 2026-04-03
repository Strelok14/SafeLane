namespace SecureFileTransfer.Core.Models;

public sealed class NodeAnnouncement
{
    public required string NodeId { get; init; }
    public required string NodeName { get; init; }
    public required string BaseUrl { get; init; }
    public long TimestampUnixMs { get; init; }
}
