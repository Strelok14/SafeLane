namespace SecureFileTransfer.App.Infrastructure;

public sealed class AppSettings
{
    public string NodeId { get; set; } = $"node-{Guid.NewGuid():N}";
    public string NodeName { get; set; } = Environment.MachineName;
    public int ListenPort { get; set; } = 5077;
    public int DiscoveryPort { get; set; } = 8888;
    public string SharedSecret { get; set; } = "demo-secret-12345";
    public string ReceivedDirectory { get; set; } = Path.Combine(AppContext.BaseDirectory, "Received");
    public int ChunkSizeKb { get; set; } = 64;
    public int MaxRequestsPerSecond { get; set; } = 10;
}
