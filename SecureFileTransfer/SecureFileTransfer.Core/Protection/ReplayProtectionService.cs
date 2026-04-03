using System.Collections.Concurrent;

namespace SecureFileTransfer.Core.Protection;

public sealed class ReplayProtectionService : IReplayProtectionService
{
    private sealed class SenderState
    {
        public ulong LastSequence;
        public long LastSeenMs;
        public readonly object Lock = new();
    }

    private readonly ConcurrentDictionary<string, SenderState> _senders = new();
    private long _lastEvictionMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    // Eviction: run every 60 s, remove senders idle for 2 min.
    // Safe because the timestamp window (5 s) still blocks replays for re-admitted senders.
    private const long EvictionIntervalMs = 60_000;
    private const long StaleSenderMs = 120_000;

    public bool IsFresh(string senderId, ulong sequenceNumber, long timestampUnixMs, int windowSeconds = 5)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Primary time-window guard (blocks all replays from outside the window)
        if (Math.Abs(now - timestampUnixMs) > windowSeconds * 1000L)
        {
            return false;
        }

        TryEvictStale(now);

        var state = _senders.GetOrAdd(senderId, _ => new SenderState { LastSequence = 0, LastSeenMs = now });

        lock (state.Lock)
        {
            if (sequenceNumber <= state.LastSequence)
            {
                return false;
            }

            state.LastSequence = sequenceNumber;
            state.LastSeenMs = now;
            return true;
        }
    }

    private void TryEvictStale(long nowMs)
    {
        var last = Interlocked.Read(ref _lastEvictionMs);
        if (nowMs - last < EvictionIntervalMs)
        {
            return;
        }

        // Only one thread performs eviction per interval
        if (Interlocked.CompareExchange(ref _lastEvictionMs, nowMs, last) != last)
        {
            return;
        }

        var cutoff = nowMs - StaleSenderMs;
        foreach (var (key, state) in _senders)
        {
            long lastSeen;
            lock (state.Lock)
            {
                lastSeen = state.LastSeenMs;
            }

            if (lastSeen < cutoff)
            {
                _senders.TryRemove(key, out _);
            }
        }
    }
}
