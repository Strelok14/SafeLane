namespace SecureFileTransfer.Core.Transfer;

public sealed class ChunkProcessResult
{
    public bool Completed { get; init; }
    public string? SavedFilePath { get; init; }
}
