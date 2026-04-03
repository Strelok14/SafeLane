@echo off
setlocal

set ROOT=%~dp0
cd /d "%ROOT%.."

echo Starting SecureFileTransfer.App...
dotnet run --project "SecureFileTransfer\SecureFileTransfer.App\SecureFileTransfer.App.csproj"
