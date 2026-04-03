using System.Collections.Concurrent;
using SecureFileTransfer.Core.Models;

namespace SecureFileTransfer.Core.Transfer;

public sealed class FileChunkAssembler : IFileChunkAssembler
{
    private sealed class SessionState
    {
        public required string SessionId { get; init; }
        public required string FileName { get; init; }
        public required string SessionPath { get; init; }
        public required bool[] ReceivedFlags { get; init; }
        public int ReceivedCount;
        public object SyncRoot { get; } = new();
    }

    private readonly ConcurrentDictionary<string, SessionState> _sessions = new();

    public async Task<ChunkProcessResult> AddChunkAsync(UploadChunkRequest request, string receivedRootPath, CancellationToken cancellationToken)
    {
        if (request.TotalChunks <= 0 || request.ChunkIndex < 0 || request.ChunkIndex >= request.TotalChunks)
        {
            throw new InvalidOperationException("Invalid chunk metadata.");
        }

        var safeFileName = Path.GetFileName(request.FileName);
        var sessionPath = Path.Combine(receivedRootPath, "_sessions", request.SessionId);

        var session = _sessions.GetOrAdd(request.SessionId, _ => new SessionState
        {
            SessionId = request.SessionId,
            FileName = safeFileName,
            SessionPath = sessionPath,
            ReceivedFlags = new bool[request.TotalChunks]
        });

        Directory.CreateDirectory(session.SessionPath);
        Directory.CreateDirectory(receivedRootPath);

        var chunkPath = Path.Combine(session.SessionPath, $"chunk_{request.ChunkIndex:D8}.bin");
        var chunkBytes = Convert.FromBase64String(request.DataBase64);
        await File.WriteAllBytesAsync(chunkPath, chunkBytes, cancellationToken);

        var shouldAssemble = false;

        lock (session.SyncRoot)
        {
            if (!session.ReceivedFlags[request.ChunkIndex])
            {
                session.ReceivedFlags[request.ChunkIndex] = true;
                session.ReceivedCount++;
            }

            shouldAssemble = session.ReceivedCount == session.ReceivedFlags.Length;
        }

        if (!shouldAssemble)
        {
            return new ChunkProcessResult { Completed = false };
        }

        var finalPath = BuildUniqueFilePath(receivedRootPath, safeFileName);

        await using (var output = File.Create(finalPath))
        {
            for (var i = 0; i < session.ReceivedFlags.Length; i++)
            {
                var part = Path.Combine(session.SessionPath, $"chunk_{i:D8}.bin");
                await using var input = File.OpenRead(part);
                await input.CopyToAsync(output, cancellationToken);
            }
        }

        try
        {
            Directory.Delete(session.SessionPath, recursive: true);
        }
        catch
        {
            // Best-effort cleanup.
        }

        _sessions.TryRemove(request.SessionId, out _);

        return new ChunkProcessResult
        {
            Completed = true,
            SavedFilePath = finalPath
        };
    }

    private static string BuildUniqueFilePath(string folder, string fileName)
    {
        var path = Path.Combine(folder, fileName);
        if (!File.Exists(path))
        {
            return path;
        }

        var name = Path.GetFileNameWithoutExtension(fileName);
        var ext = Path.GetExtension(fileName);

        for (var i = 1; i <= 9999; i++)
        {
            var candidate = Path.Combine(folder, $"{name}_{i}{ext}");
            if (!File.Exists(candidate))
            {
                return candidate;
            }
        }

        return Path.Combine(folder, $"{name}_{Guid.NewGuid():N}{ext}");
    }
}
