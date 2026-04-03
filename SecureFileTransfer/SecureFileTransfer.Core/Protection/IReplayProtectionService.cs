namespace SecureFileTransfer.Core.Protection;

public interface IReplayProtectionService
{
    bool IsFresh(string senderId, ulong sequenceNumber, long timestampUnixMs, int windowSeconds = 5);
}
