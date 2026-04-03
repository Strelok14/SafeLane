namespace SecureFileTransfer.App.ViewModels;

public sealed class NodeViewModel
{
    public required string NodeId { get; init; }
    public required string Name { get; set; }
    public required string BaseUrl { get; set; }
    public bool IsTrusted { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastSeenUtc { get; set; }

    public string Status => IsOnline ? "Online" : "Offline";
}
