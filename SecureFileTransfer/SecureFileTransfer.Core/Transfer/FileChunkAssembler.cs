using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using SecureFileTransfer.Core.Models;

namespace SecureFileTransfer.Core.Transfer;

public sealed class FileChunkAssembler : IFileChunkAssembler
{
    // SessionId is Guid.NewGuid().ToString("N") — exactly 32 lowercase hex characters.
    // Enforcing this prevents path traversal via crafted SessionId values.
    private static readonly Regex SafeSessionIdRegex =
        new(@"^[a-f0-9]{32}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    // Upper bound on concurrent in-flight sessions to prevent disk/memory exhaustion.
    private const int MaxConcurrentSessions = 50;

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

    public async Task<ChunkProcessResult> AddChunkAsync(UploadChunkRequest request, byte[] chunkData, string receivedRootPath, CancellationToken cancellationToken)
    {
        if (request.TotalChunks <= 0 || request.ChunkIndex < 0 || request.ChunkIndex >= request.TotalChunks)
        {
            throw new InvalidOperationException("Некорректные метаданные чанка.");
        }

        // Path-traversal guard: SessionId must be a canonical 32-char hex GUID string.
        if (string.IsNullOrEmpty(request.SessionId) || !SafeSessionIdRegex.IsMatch(request.SessionId))
        {
            throw new InvalidOperationException("Недопустимый идентификатор сессии.");
        }

        var safeFileName = Path.GetFileName(request.FileName);
        if (string.IsNullOrEmpty(safeFileName))
        {
            throw new InvalidOperationException("Недопустимое имя файла в метаданных чанка.");
        }

        // Concurrent session limit to prevent disk/memory flooding.
        var sessionPath = Path.Combine(receivedRootPath, "_sessions", request.SessionId);

        // Track whether this GetOrAdd creates a new session so we can enforce the limit atomically.
        var isNewSession = false;
        var session = _sessions.GetOrAdd(request.SessionId, _ =>
        {
            isNewSession = true;
            return new SessionState
            {
                SessionId = request.SessionId,
                FileName = safeFileName,
                SessionPath = sessionPath,
                ReceivedFlags = new bool[request.TotalChunks]
            };
        });

        // If this thread just added a new session that pushed us over the limit, roll it back.
        if (isNewSession && _sessions.Count > MaxConcurrentSessions)
        {
            _sessions.TryRemove(request.SessionId, out _);
            throw new InvalidOperationException($"Превышен лимит параллельных сессий ({MaxConcurrentSessions}).");
        }

        Directory.CreateDirectory(session.SessionPath);
        Directory.CreateDirectory(receivedRootPath);

        var chunkPath = Path.Combine(session.SessionPath, $"chunk_{request.ChunkIndex:D8}.bin");
        await File.WriteAllBytesAsync(chunkPath, chunkData, cancellationToken);

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
