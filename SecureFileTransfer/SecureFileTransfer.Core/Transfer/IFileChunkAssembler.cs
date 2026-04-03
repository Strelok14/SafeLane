using SecureFileTransfer.Core.Models;

namespace SecureFileTransfer.Core.Transfer;

public interface IFileChunkAssembler
{
    Task<ChunkProcessResult> AddChunkAsync(UploadChunkRequest request, string receivedRootPath, CancellationToken cancellationToken);
}
