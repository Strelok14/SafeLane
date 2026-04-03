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

    // Remove counters that belong to a past second (entries can't be reused anyway).
    // Called on every request — negligible cost because the condition is rarely true.
    private void TryCleanup(long nowSecond)
    {
        if (_counters.Count < 500)
        {
            return;
        }

        foreach (var (key, counter) in _counters)
        {
            if (counter.WindowSecond < nowSecond - 2)
            {
                _counters.TryRemove(key, out _);
            }
        }
    }

    public bool IsAllowed(string key, int maxPerSecond = 10)
    {
        var nowSecond = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        TryCleanup(nowSecond);

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
