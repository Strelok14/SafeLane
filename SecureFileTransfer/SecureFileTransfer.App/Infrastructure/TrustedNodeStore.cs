using System.Text.Json;
using SecureFileTransfer.Core.Models;

namespace SecureFileTransfer.App.Infrastructure;

public sealed class TrustedNodeStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _path;

    public TrustedNodeStore(string appDataRoot)
    {
        Directory.CreateDirectory(appDataRoot);
        _path = Path.Combine(appDataRoot, "trusted_nodes.json");
    }

    public List<TrustedNodeConfig> Load()
    {
        if (!File.Exists(_path))
        {
            Save(new List<TrustedNodeConfig>());
            return new List<TrustedNodeConfig>();
        }

        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<List<TrustedNodeConfig>>(json, JsonOptions) ?? new List<TrustedNodeConfig>();
    }

    public void Save(IReadOnlyCollection<TrustedNodeConfig> nodes)
    {
        var json = JsonSerializer.Serialize(nodes, JsonOptions);
        File.WriteAllText(_path, json);
    }
}
