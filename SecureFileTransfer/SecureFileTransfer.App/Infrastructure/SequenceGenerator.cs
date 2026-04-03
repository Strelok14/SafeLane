namespace SecureFileTransfer.App.Infrastructure;

public sealed class SequenceGenerator
{
    // Seed from current Unix-ms so that sequence is monotonically increasing even after restart.
    // This prevents the ReplayProtectionService on the remote side from rejecting post-restart messages.
    private ulong _value = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    private readonly object _sync = new();

    public ulong Next()
    {
        lock (_sync)
        {
            _value++;
            return _value;
        }
    }
}
