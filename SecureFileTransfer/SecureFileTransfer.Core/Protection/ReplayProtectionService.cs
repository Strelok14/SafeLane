using System.Collections.Concurrent;

namespace SecureFileTransfer.Core.Protection;

public sealed class ReplayProtectionService : IReplayProtectionService
{
    private readonly ConcurrentDictionary<string, ulong> _lastSequenceBySender = new();

    public bool IsFresh(string senderId, ulong sequenceNumber, long timestampUnixMs, int windowSeconds = 5)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var delta = Math.Abs(now - timestampUnixMs);
        if (delta > windowSeconds * 1000L)
        {
            return false;
        }

        while (true)
        {
            if (!_lastSequenceBySender.TryGetValue(senderId, out var last))
            {
                if (_lastSequenceBySender.TryAdd(senderId, sequenceNumber))
                {
                    return true;
                }

                continue;
            }

            if (sequenceNumber <= last)
            {
                return false;
            }

            if (_lastSequenceBySender.TryUpdate(senderId, sequenceNumber, last))
            {
                return true;
            }
        }
    }
}
