namespace SecureFileTransfer.App.Infrastructure;

public sealed class SequenceGenerator
{
    private ulong _value;
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
