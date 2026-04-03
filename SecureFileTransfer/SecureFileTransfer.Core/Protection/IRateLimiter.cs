namespace SecureFileTransfer.Core.Protection;

public interface IRateLimiter
{
    bool IsAllowed(string key, int maxPerSecond = 10);
}
