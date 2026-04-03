namespace SecureFileTransfer.Core.Models;

public sealed class UploadChunkRequest
{
    public required string SenderId { get; init; }
    public required string SessionId { get; init; }
    public required string FileName { get; init; }
    public long FileSize { get; init; }
    public int ChunkIndex { get; init; }
    public int TotalChunks { get; init; }
    public ulong SequenceNumber { get; init; }
    public long TimestampUnixMs { get; init; }
    public required string DataBase64 { get; init; }
    public required string SignatureBase64 { get; init; }
}
