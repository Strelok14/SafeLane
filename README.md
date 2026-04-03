# SafeLane

**Standalone .NET 8 application for secure file transfer over local network (LAN) between trusted nodes.**

Secure point-to-point file transfer for tactical networks without internet connectivity. Built with Windows Forms UI, embedded HTTP server, and military-grade HMAC-SHA256 packet authentication.

## Key Features

- ✅ **Standalone executable** - No installation required, portable across Windows machines
- 🔐 **HMAC-SHA256 security** - Every packet (handshake + file chunk) is cryptographically signed
- 🛡️ **Replay protection** - Timestamp + sequence number prevents duplicate packet attacks
- ⚡ **Rate limiting** - Per-IP request throttling (default 10 req/sec, configurable)
- 🤝 **Trusted node model** - Manually curated list of known peers with persistent configuration
- 📡 **UDP multicast discovery** - Automatic peer detection on local network (239.0.0.1:8888)
- 📦 **Chunked transfer** - 64 KB default chunks with automatic reassembly and collision avoidance
- 🖥️ **Windows Forms UI** - Simple, no-frills interface for file selection and transfer control
- 💾 **Embedded Kestrel server** - HTTP server built-in, no external service needed
- 🔄 **Hybrid P2P model** - Every node is both server and client simultaneously

## Security Model

**Based on GOST-inspired cryptographic approach with the following components:**

- **Authentication**: HMAC-SHA256 over canonical message representation
- **Key derivation**: PBKDF2-HMAC-SHA256 with 150,000 iterations → 32-byte symmetric key
- **Replay protection**: Per-sender sequence number + 5-second timestamp window
- **Constant-time comparison**: Signature verification immune to timing attacks
- **Per-message overhead**: ~100 bytes for signature + metadata per transfer

**Why no TLS?**
- Trusted LAN assumption (same network segment)
- HMAC provides authentication + integrity without encryption overhead
- Simplified certificate management
- Single shared secret per deployment

## Solution Structure

### Core Library (`SecureFileTransfer.Core`)
- **Models**: Packet contracts (handshake, upload, discovery)
- **Security**: HMAC computation, key derivation, message canonicalization
- **Protection**: Replay prevention, rate limiting
- **Transfer**: Chunked file assembly with collision handling

### Application (`SecureFileTransfer.App`)
- **UI**: Windows Forms with peer grid, file selector, progress tracking
- **Networking**: Embedded Kestrel HTTP server + UDP discovery service
- **Infrastructure**: Settings persistence, trusted node management, sequence generation
- **Client**: File chunking and retry logic

## Quick Start

### Windows

```powershell
cd SecureFileTransfer
.\run.bat
```

### Linux/macOS (via .NET)

```bash
cd SecureFileTransfer
chmod +x ./run.sh
./run.sh
```

### Manual Build & Run

```bash
dotnet build SecureFileTransfer.sln
dotnet run --project ./SecureFileTransfer/SecureFileTransfer.App
```

## Usage Workflow

1. **Application starts**
   - Embedded HTTP server listens on `http://localhost:5077`
   - UDP multicast listener (239.0.0.1:8888) announces presence every 5 seconds

2. **Add trusted node**
   - Button: "Add Node"
   - Input: `node_id` (e.g., "field-unit-3"), `name`, `base_url`, optional shared secret override
   - Node appears in grid with "Online" status when reachable

3. **Send file**
   - Select target node from grid (or newly discovered peer)
   - Click "Choose file" and select document/data
   - Click "Send" → progress bar shows transfer status
   - File reassembled in `Received/` folder on recipient

4. **Configure settings** (optional)
   - Button: "Settings"
   - Adjust: node name, listen port, shared secret, chunk size, rate limit, received folder

## Configuration

Settings stored in `App_Data/settings.json`:

```json
{
  "node_id": "unique-identifier",
  "node_name": "Field Unit Alpha",
  "listen_port": 5077,
  "discovery_port": 8888,
  "shared_secret": "your-shared-secret-here",
  "received_directory": "./Received/",
  "chunk_size_kb": 64,
  "max_requests_per_second": 10
}
```

Trusted nodes stored in `App_Data/trusted_nodes.json`:

```json
[
  {
    "node_id": "bravo-unit",
    "display_name": "Bravo Platoon",
    "base_url": "http://192.168.1.50:5077",
    "optional_secret_override": null
  }
]
```

## Technical Specifications

| Component | Specification |
|-----------|---------------|
| **Framework** | .NET 8 |
| **UI** | Windows Forms |
| **HTTP Server** | ASP.NET Core Kestrel (embedded) |
| **Transport** | HTTP POST (cleartext, HMAC-signed) |
| **Discovery** | UDP multicast (239.0.0.1:8888) every 5s |
| **Default listen port** | 5077 |
| **Auth port** | 8888 |
| **Default chunk size** | 64 KB |
| **Timestamp window** | ±5 seconds |
| **Rate limit** | 10 req/sec per IP |
| **Key length** | 256 bits (PBKDF2 output) |
| **Signature** | Base64(HMAC-SHA256) |

## API Endpoints

### POST /api/handshake
Pre-transfer identity verification.

**Request:**
```json
{
  "sender_id": "unit-1",
  "sequence_number": 1,
  "timestamp_ms": 1704067200000,
  "signature_base64": "SGVsbG8gV29ybGQ..."
}
```

**Response:** 200 OK or 403 Forbidden

### POST /api/upload
File chunk upload.

**Request:**
```json
{
  "sender_id": "unit-1",
  "session_id": "abc123",
  "file_name": "document.pdf",
  "file_size": 2097152,
  "chunk_index": 0,
  "total_chunks": 32,
  "sequence_number": 2,
  "timestamp_ms": 1704067201000,
  "data_base64": "SGVsbG8gV29ybGQ...",
  "signature_base64": "aGVsbG8gd29ybGQ..."
}
```

**Response:**
```json
{
  "completed": false,
  "saved_file_path": null
}
```

If all chunks received:
```json
{
  "completed": true,
  "saved_file_path": "Received/document.pdf"
}
```

### GET /health
Liveness check.

**Response:** 200 OK

## Hardening for Production

Recommendations for high-security deployments:

1. **Per-node unique keys** instead of shared secret
2. **Persistent replay state** (disk-backed journal)
3. **TLS + certificate pinning** for untrusted networks
4. **Resumable transfers** with integrity manifest (SHA-256 per file)
5. **Multi-hop relay** support for extending range
6. **Integrity verification** post-transfer (MD5 or SHA-256 manifest)
7. **Audit logging** with cryptographic signing

## Troubleshooting

| Issue | Solution |
|-------|----------|
| **Nodes not discovering** | Check firewall allows UDP 8888 multicast; ensure same subnet |
| **"Rate limit exceeded" error** | Increase `max_requests_per_second` in settings; check target overload |
| **"Replay protection triggered"** | Verify system clocks synchronized (NTP); check sequence number state |
| **Transfer hangs mid-file** | Check network stability; verify receiver has disk space in `received_directory` |
| **"Signature verification failed"** | Confirm shared secret matches on sender + receiver |

## License

Internal Use Only

## Author

TACSIT SafeLane Project
