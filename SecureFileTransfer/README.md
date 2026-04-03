# SecureFileTransfer

Standalone .NET 8 application for secure file transfer over local network between trusted nodes.

## Security Model (based on requested method)

- HMAC-SHA256 for every packet (handshake + each file chunk)
- Replay protection (`timestamp + sequence_number`) in memory
- Fixed-window rate limiting per source IP (default `10 req/sec`)
- Trusted nodes model (manually added `node_id + base_url + optional secret override`)
- Shared secret -> 256-bit key via PBKDF2

## Solution Structure

- `SecureFileTransfer.Core`
  - models and packet contracts
  - HMAC / key derivation
  - replay and rate limiting services
  - file chunk assembler
- `SecureFileTransfer.App`
  - Windows Forms UI
  - embedded Kestrel server (`/api/handshake`, `/api/upload`)
  - UDP multicast node discovery (`239.0.0.1:8888`)
  - file sender logic with chunking and retries

## Build

From repository root (`StrikeballServer`):

```powershell
dotnet build .\SecureFileTransfer.sln
```

## Run

### Windows

```powershell
.\SecureFileTransfer\run.bat
```

### Shell script

```bash
chmod +x ./SecureFileTransfer/run.sh
./SecureFileTransfer/run.sh
```

## Usage

1. Start app on both machines in same LAN.
2. Open **Settings** and set the same shared secret on both sides.
3. Add each machine as a trusted node on the other machine (Node ID + Base URL).
4. Choose target node in grid.
5. Choose file.
6. Click **Send**.
7. Received files are stored in `Received` directory (configurable).

## Packet Fields

Upload chunk payload includes:

- `sender_id`
- `session_id`
- `file_name`
- `file_size`
- `chunk_index`
- `total_chunks`
- `sequence_number`
- `timestamp`
- `data_base64`
- `signature_base64`

Canonical string for HMAC:

`sender_id|session_id|file_name|file_size|chunk_index|total_chunks|sequence_number|timestamp|data_base64`

## Offline Notes

- Works without internet.
- Requires .NET 8 runtime/SDK on the machine.
- App settings and trusted nodes are stored in `SecureFileTransfer.App/bin/.../App_Data` while running from source.

## Security Limitations of demo version

- Shared secret can be common for all nodes (simplified trust model).
- Replay/rate limiter are in-memory (reset on app restart).
- No TLS by design for trusted LAN scenario.

## Suggested production hardening

- per-node unique keys and secure key exchange workflow
- persistent replay state (Redis)
- TLS + certificate pinning
- resumable transfers with integrity manifest for large files
