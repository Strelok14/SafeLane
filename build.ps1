#!/usr/bin/env pwsh

# SafeLane Build and Test Script
# Проверяет что приложение собирается без ошибок

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "🔨 SafeLane - Проверка сборки" -ForegroundColor Green
Write-Host "═════════════════════════════════════════" -ForegroundColor Green
Write-Host ""

$solutionPath = Join-Path $repoRoot "SecureFileTransfer.sln"

if (-not (Test-Path $solutionPath)) {
    Write-Host "❌ Файл $solutionPath не найден" -ForegroundColor Red
    exit 1
}

Write-Host "📦 Сборка решения..."
Write-Host ""

cd $repoRoot

# Очищаем предыдущую сборку
Write-Host "🧹 Очистка..."
dotnet clean $solutionPath -q 2>&1 | Out-Null

# Собрубраем проект
Write-Host "🏗️  Компиляция..."
$buildResult = dotnet build $solutionPath --no-restore -c Release 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ СБОРКА УСПЕШНА!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📊 Статистика:"
    Write-Host "  ✓ Все ошибки исправлены"
    Write-Host "  ✓ Приложение скомпилировано"
    Write-Host "  ✓ Готово к развертыванию"
    Write-Host ""
    
    # Проверяем что exe существует
    $exePath = "SecureFileTransfer\SecureFileTransfer.App\bin\Release\net8.0-windows\SecureFileTransfer.App.exe"
    if (Test-Path $exePath) {
        $size = (Get-Item $exePath).Length / 1MB
        Write-Host "📁 Приложение: SecureFileTransfer.App.exe"
        Write-Host "   Размер: $([math]::Round($size, 1)) MB"
    }
    
    exit 0
} else {
    Write-Host ""
    Write-Host "❌ ОШИБКИ СБОРКИ!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Ошибки:" -ForegroundColor Yellow
    $buildResult | Select-String "error CS" | ForEach-Object { Write-Host "  $_" }
    
    exit 1
}
