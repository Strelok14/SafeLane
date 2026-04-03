using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using SecureFileTransfer.Core.Models;

namespace SecureFileTransfer.App.Networking;

public sealed class NodeDiscoveryService : IDisposable
{
    private readonly string _nodeId;
    private readonly string _nodeName;
    private readonly int _discoveryPort;
    private readonly Func<int> _listenPortFactory;
    private readonly Action<string> _log;
    private readonly CancellationTokenSource _cts = new();
    private UdpClient? _sender;
    private UdpClient? _receiver;

    public event Action<NodeAnnouncement, IPEndPoint>? NodeSeen;

    public NodeDiscoveryService(string nodeId, string nodeName, int discoveryPort, Func<int> listenPortFactory, Action<string> log)
    {
        _nodeId = nodeId;
        _nodeName = nodeName;
        _discoveryPort = discoveryPort;
        _listenPortFactory = listenPortFactory;
        _log = log;
    }

    public void Start()
    {
        _sender = new UdpClient();

        _receiver = new UdpClient(AddressFamily.InterNetwork);
        _receiver.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _receiver.Client.Bind(new IPEndPoint(IPAddress.Any, _discoveryPort));
        _receiver.JoinMulticastGroup(IPAddress.Parse("239.0.0.1"));

        _ = Task.Run(BroadcastLoopAsync);
        _ = Task.Run(ReceiveLoopAsync);
    }

    private async Task BroadcastLoopAsync()
    {
        var multicast = new IPEndPoint(IPAddress.Parse("239.0.0.1"), _discoveryPort);

        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var ann = new NodeAnnouncement
                {
                    NodeId = _nodeId,
                    NodeName = _nodeName,
                    BaseUrl = $"http://{GetLocalIp()}:{_listenPortFactory()}",
                    TimestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

                var json = JsonSerializer.Serialize(ann);
                var bytes = Encoding.UTF8.GetBytes(json);
                await _sender!.SendAsync(bytes, bytes.Length, multicast);
            }
            catch (Exception ex)
            {
                _log($"Discovery broadcast error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), _cts.Token).ContinueWith(_ => { });
        }
    }

    private async Task ReceiveLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var result = await _receiver!.ReceiveAsync(_cts.Token);

                // Discard oversized packets before any deserialization (simple DoS guard).
                const int maxAnnouncementBytes = 1024;
                if (result.Buffer.Length > maxAnnouncementBytes)
                {
                    continue;
                }

                var json = Encoding.UTF8.GetString(result.Buffer);
                var ann = JsonSerializer.Deserialize<NodeAnnouncement>(json);
                if (ann is null || ann.NodeId == _nodeId)
                {
                    continue;
                }

                // Discard stale announcements (older than 15 seconds) to prevent replay of discovery packets.
                var ageMs = Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - ann.TimestampUnixMs);
                if (ageMs > 15_000)
                {
                    continue;
                }

                NodeSeen?.Invoke(ann, result.RemoteEndPoint);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _log($"Discovery receive error: {ex.Message}");
            }
        }
    }

    private static string GetLocalIp()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
        return ip?.ToString() ?? "127.0.0.1";
    }

    public void Dispose()
    {
        _cts.Cancel();
        _receiver?.Dispose();
        _sender?.Dispose();
        _cts.Dispose();
    }
}
