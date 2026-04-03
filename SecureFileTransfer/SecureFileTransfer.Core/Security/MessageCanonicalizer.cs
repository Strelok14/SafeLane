using SecureFileTransfer.Core.Models;

namespace SecureFileTransfer.Core.Security;

public static class MessageCanonicalizer
{
    public static string ForHandshake(HandshakeRequest request)
    {
        return string.Join('|',
            request.SenderId,
            request.Nonce,
            request.SequenceNumber,
            request.TimestampUnixMs);
    }

    public static string ForUpload(UploadChunkRequest request)
    {
        return string.Join('|',
            request.SenderId,
            request.SessionId,
            request.FileName,
            request.FileSize,
            request.ChunkIndex,
            request.TotalChunks,
            request.SequenceNumber,
            request.TimestampUnixMs,
            request.DataBase64);
    }
}
