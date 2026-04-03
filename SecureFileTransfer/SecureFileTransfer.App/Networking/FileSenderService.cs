using System.Net.Http.Json;
using SecureFileTransfer.App.Infrastructure;
using SecureFileTransfer.Core.Models;
using SecureFileTransfer.Core.Security;

namespace SecureFileTransfer.App.Networking;

public sealed class FileSenderService
{
    private readonly AppSettings _settings;
    private readonly SequenceGenerator _sequence;
    private readonly IKeyDerivationService _keyDerivation = new KeyDerivationService();
    private readonly IHmacSignatureService _hmac = new HmacSignatureService();
    private readonly HttpClient _httpClient = new();
    private readonly Action<string> _log;

    public FileSenderService(AppSettings settings, SequenceGenerator sequence, Action<string> log)
    {
        _settings = settings;
        _sequence = sequence;
        _log = log;
    }

    public async Task SendFileAsync(string filePath, TrustedNodeConfig target, CancellationToken cancellationToken, Action<int, int> onProgress)
    {
        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        var chunkSize = Math.Max(16 * 1024, _settings.ChunkSizeKb * 1024);
        var totalChunks = (int)Math.Ceiling((double)fileInfo.Length / chunkSize);
        var sessionId = Guid.NewGuid().ToString("N");

        var secret = string.IsNullOrWhiteSpace(target.SharedSecretOverride) ? _settings.SharedSecret : target.SharedSecretOverride;
        var key = _keyDerivation.DeriveKeyBytes(secret!);

        await PerformHandshakeAsync(target, key, cancellationToken);

        await using var stream = File.OpenRead(filePath);
        var buffer = new byte[chunkSize];

        for (var chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var read = await stream.ReadAsync(buffer.AsMemory(0, chunkSize), cancellationToken);
            if (read <= 0)
            {
                break;
            }

            var chunkData = Convert.ToBase64String(buffer, 0, read);
            var request = new UploadChunkRequest
            {
                SenderId = _settings.NodeId,
                SessionId = sessionId,
                FileName = fileInfo.Name,
                FileSize = fileInfo.Length,
                ChunkIndex = chunkIndex,
                TotalChunks = totalChunks,
                SequenceNumber = _sequence.Next(),
                TimestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                DataBase64 = chunkData,
                SignatureBase64 = string.Empty
            };

            var canonical = MessageCanonicalizer.ForUpload(request);
            request = new UploadChunkRequest
            {
                SenderId = request.SenderId,
                SessionId = request.SessionId,
                FileName = request.FileName,
                FileSize = request.FileSize,
                ChunkIndex = request.ChunkIndex,
                TotalChunks = request.TotalChunks,
                SequenceNumber = request.SequenceNumber,
                TimestampUnixMs = request.TimestampUnixMs,
                DataBase64 = request.DataBase64,
                SignatureBase64 = _hmac.ComputeBase64(canonical, key)
            };

            var attempts = 0;
            while (true)
            {
                attempts++;
                var response = await _httpClient.PostAsJsonAsync($"{target.BaseUrl.TrimEnd('/')}/api/upload", request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    break;
                }

                if (attempts >= 3)
                {
                    var text = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new InvalidOperationException($"Chunk {chunkIndex} failed: {(int)response.StatusCode} {text}");
                }

                await Task.Delay(150, cancellationToken);
            }

            onProgress(chunkIndex + 1, totalChunks);
            await Task.Delay(100, cancellationToken);
        }

        _log($"File sent to {target.DisplayName}: {fileInfo.Name} ({fileInfo.Length} bytes)");
    }

    private async Task PerformHandshakeAsync(TrustedNodeConfig target, byte[] key, CancellationToken cancellationToken)
    {
        var request = new HandshakeRequest
        {
            SenderId = _settings.NodeId,
            Nonce = Guid.NewGuid().ToString("N"),
            SequenceNumber = _sequence.Next(),
            TimestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            SignatureBase64 = string.Empty
        };

        var canonical = MessageCanonicalizer.ForHandshake(request);
        request = new HandshakeRequest
        {
            SenderId = request.SenderId,
            Nonce = request.Nonce,
            SequenceNumber = request.SequenceNumber,
            TimestampUnixMs = request.TimestampUnixMs,
            SignatureBase64 = _hmac.ComputeBase64(canonical, key)
        };

        var response = await _httpClient.PostAsJsonAsync($"{target.BaseUrl.TrimEnd('/')}/api/handshake", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
