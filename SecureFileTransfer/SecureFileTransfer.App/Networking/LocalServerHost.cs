using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecureFileTransfer.App.Infrastructure;
using SecureFileTransfer.Core.Models;
using SecureFileTransfer.Core.Protection;
using SecureFileTransfer.Core.Security;
using SecureFileTransfer.Core.Transfer;

namespace SecureFileTransfer.App.Networking;

public sealed class LocalServerHost : IAsyncDisposable
{
    private readonly AppSettings _settings;
    private readonly Func<IReadOnlyCollection<TrustedNodeConfig>> _trustedNodesProvider;
    private readonly Action<string> _log;
    private readonly Action<int, int> _receiveProgress;
    private readonly IRateLimiter _rateLimiter = new FixedWindowRateLimiter();
    private readonly IReplayProtectionService _replay = new ReplayProtectionService();
    private readonly IKeyDerivationService _keyDerivation = new KeyDerivationService();
    private readonly IHmacSignatureService _hmac = new HmacSignatureService();
    private readonly IFileChunkAssembler _assembler = new FileChunkAssembler();
    private WebApplication? _app;

    public LocalServerHost(
        AppSettings settings,
        Func<IReadOnlyCollection<TrustedNodeConfig>> trustedNodesProvider,
        Action<string> log,
        Action<int, int> receiveProgress)
    {
        _settings = settings;
        _trustedNodesProvider = trustedNodesProvider;
        _log = log;
        _receiveProgress = receiveProgress;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = Array.Empty<string>(),
            ApplicationName = typeof(LocalServerHost).Assembly.GetName().Name,
            ContentRootPath = AppContext.BaseDirectory
        });

        builder.WebHost.UseKestrel(options => { options.ListenAnyIP(_settings.ListenPort); });
        builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.PropertyNameCaseInsensitive = true; });
        builder.Services.AddSingleton(_settings);
        builder.Services.AddSingleton<IRateLimiter>(_rateLimiter);

        _app = builder.Build();
        _app.UseMiddleware<RateLimitingMiddleware>();

        _app.MapPost("/api/handshake", (HttpContext context, HandshakeRequest request) =>
        {
            var node = GetTrustedNode(request.SenderId);
            if (node is null)
            {
                _log($"Handshake rejected: unknown sender {request.SenderId}");
                return Results.Unauthorized();
            }

            if (!_replay.IsFresh(request.SenderId, request.SequenceNumber, request.TimestampUnixMs))
            {
                _log($"Replay detected on handshake from {request.SenderId}");
                return Results.BadRequest(new HandshakeResponse { Success = false, Message = "Replay detected." });
            }

            var secret = string.IsNullOrWhiteSpace(node.SharedSecretOverride) ? _settings.SharedSecret : node.SharedSecretOverride;
            var key = _keyDerivation.DeriveKeyBytes(secret!);
            var canonical = MessageCanonicalizer.ForHandshake(request);

            if (!_hmac.VerifyBase64(canonical, key, request.SignatureBase64))
            {
                _log($"Handshake signature invalid from {request.SenderId}");
                return Results.BadRequest(new HandshakeResponse { Success = false, Message = "Invalid signature." });
            }

            return Results.Ok(new HandshakeResponse { Success = true, Message = "Handshake accepted." });
        });

        _app.MapPost("/api/upload", async (HttpContext context, UploadChunkRequest request, CancellationToken ct) =>
        {
            var node = GetTrustedNode(request.SenderId);
            if (node is null)
            {
                _log($"Upload rejected: unknown sender {request.SenderId}");
                return Results.Unauthorized();
            }

            if (!_replay.IsFresh(request.SenderId, request.SequenceNumber, request.TimestampUnixMs))
            {
                _log($"Replay detected on upload from {request.SenderId}, session {request.SessionId}");
                return Results.BadRequest("Replay detected.");
            }

            var secret = string.IsNullOrWhiteSpace(node.SharedSecretOverride) ? _settings.SharedSecret : node.SharedSecretOverride;
            var key = _keyDerivation.DeriveKeyBytes(secret!);
            var canonical = MessageCanonicalizer.ForUpload(request);
            if (!_hmac.VerifyBase64(canonical, key, request.SignatureBase64))
            {
                _log($"Invalid HMAC for chunk {request.ChunkIndex}/{request.TotalChunks - 1} from {request.SenderId}");
                return Results.BadRequest("Invalid signature.");
            }

            var result = await _assembler.AddChunkAsync(request, _settings.ReceivedDirectory, ct);
            _receiveProgress(request.ChunkIndex + 1, request.TotalChunks);

            if (result.Completed)
            {
                _log($"File received: {result.SavedFilePath}");
            }

            return Results.Ok(new
            {
                ok = true,
                completed = result.Completed,
                path = result.SavedFilePath
            });
        });

        _app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

        Directory.CreateDirectory(_settings.ReceivedDirectory);
        await _app.StartAsync(cancellationToken);
        _log($"Embedded server started on http://0.0.0.0:{_settings.ListenPort}");
    }

    private TrustedNodeConfig? GetTrustedNode(string senderId)
    {
        return _trustedNodesProvider().FirstOrDefault(n => string.Equals(n.NodeId, senderId, StringComparison.OrdinalIgnoreCase));
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
            _app = null;
        }
    }
}
