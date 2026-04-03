#!/usr/bin/env pwsh

<# 
SafeLane GitHub Push Script
Этот скрипт помогает выполнить push SafeLane репозитория в GitHub

Использование:
  ./push_to_github.ps1 -Token "ваш_github_token"
  ./push_to_github.ps1 -UseSsh $true
#>

param(
    [string]$Token = $null,
    [bool]$UseSsh = $false,
    [bool]$Force = $false
)

$repoPath = Split-Path -Parent $MyInvocation.MyCommand.Path
if (-not (Test-Path "$repoPath\.git")) {
    Write-Host "❌ Это не git репозиторий. Запустите скрипт из root папки SafeLane" -ForegroundColor Red
    exit 1
}

cd $repoPath

Write-Host "🚀 SafeLane GitHub Push" -ForegroundColor Green
Write-Host "========================" -ForegroundColor Green
Write-Host ""

# Проверяем статус
Write-Host "📊 Статус репозитория:" -ForegroundColor Cyan
git status --short

Write-Host ""
Write-Host "📋 Коммиты:" -ForegroundColor Cyan
git log --oneline -3

Write-Host ""

if ($UseSsh) {
    Write-Host "🔑 Используется SSH..." -ForegroundColor Yellow
    git remote set-url origin git@github.com:Strelok14/SafeLane.git
} elseif ($Token) {
    Write-Host "🔑 Используется Personal Access Token..." -ForegroundColor Yellow
    git remote set-url origin "https://Strelok14:$Token@github.com/Strelok14/SafeLane.git"
} else {
    Write-Host "⚠️  Нет credentials. Файл готов к ручному push:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Вариант 1 - Использовать Personal Access Token:" -ForegroundColor Cyan
    Write-Host "  1. Создать token на https://github.com/settings/tokens"
    Write-Host "  2. Выполнить: .\push_to_github.ps1 -Token '<ваш_token>'" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Вариант 2 - Использовать SSH:" -ForegroundColor Cyan
    Write-Host "  1. ssh-keygen -t rsa (если нет ключей)"
    Write-Host "  2. cat ~/.ssh/id_rsa.pub | clip (скопировать public key)"
    Write-Host "  3. Добавить на https://github.com/settings/keys"
    Write-Host "  4. Выполнить: .\push_to_github.ps1 -UseSsh \$true" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Вариант 3 - Ручной push с GitHub CLI:" -ForegroundColor Cyan
    Write-Host "  gh auth login" -ForegroundColor Gray
    Write-Host "  gh auth setup-git" -ForegroundColor Gray
    Write-Host "  git push -u origin main" -ForegroundColor Gray
    exit 0
}

Write-Host ""
Write-Host "📤 Выполняю push (это может занять 30-60 секунд)..." -ForegroundColor Yellow

$pushArgs = @("-u", "origin", "main")
if ($Force) {
    $pushArgs += @("--force")
    Write-Host "   (используется --force)" -ForegroundColor Yellow
}

try {
    git push @pushArgs 2>&1 | Tee-Object -Variable output | ForEach-Object {
        Write-Host $_
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ УСПЕШНО!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Репозиторий доступен на:" -ForegroundColor Green
        Write-Host "  https://github.com/Strelok14/SafeLane" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Следующие команды для работы:" -ForegroundColor Green
        Write-Host "  git clone https://github.com/Strelok14/SafeLane.git" -ForegroundColor Gray
        Write-Host "  cd SafeLane && dotnet build SecureFileTransfer.sln" -ForegroundColor Gray
    } else {
        Write-Host ""
        Write-Host "❌ ОШИБКА при push!" -ForegroundColor Red
        Write-Host ""
        Write-Host "Решение проблем:" -ForegroundColor Yellow
        Write-Host "  1. Проверить что репозиторий существует на GitHub"
        Write-Host "  2. Проверить что token/SSH ключ правильный"
        Write-Host "  3. Проверить интернет соединение"
    }
} catch {
    Write-Host "❌ Исключение: $_" -ForegroundColor Red
}
