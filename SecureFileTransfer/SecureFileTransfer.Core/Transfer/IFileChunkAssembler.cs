using SecureFileTransfer.Core.Models;

namespace SecureFileTransfer.Core.Transfer;

public interface IFileChunkAssembler
{
    /// <param name="request">Chunk metadata (sender, session, index, etc.).</param>
    /// <param name="chunkData">Already-decrypted raw bytes for this chunk.</param>
    /// <param name="receivedRootPath">Directory where assembled files are saved.</param>
    Task<ChunkProcessResult> AddChunkAsync(UploadChunkRequest request, byte[] chunkData, string receivedRootPath, CancellationToken cancellationToken);
}
