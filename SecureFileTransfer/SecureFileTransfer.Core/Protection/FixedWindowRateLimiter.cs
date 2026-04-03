using System.Collections.Concurrent;

namespace SecureFileTransfer.Core.Protection;

public sealed class FixedWindowRateLimiter : IRateLimiter
{
    private sealed class Counter
    {
        public long WindowSecond;
        public int Count;
    }

    private readonly ConcurrentDictionary<string, Counter> _counters = new();

    public bool IsAllowed(string key, int maxPerSecond = 10)
    {
        var nowSecond = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var counter = _counters.GetOrAdd(key, _ => new Counter { WindowSecond = nowSecond, Count = 0 });

        lock (counter)
        {
            if (counter.WindowSecond != nowSecond)
            {
                counter.WindowSecond = nowSecond;
                counter.Count = 0;
            }

            counter.Count++;
            return counter.Count <= maxPerSecond;
        }
    }
}
