# Инструкция по Push в GitHub

Репозиторий SafeLane подготовлен и находится в `c:\Я\ICIDS\SafeLane`

## Шаг 1: Убедитесь что существует репозиторий на GitHub

Перейдите в https://github.com/Strelok14/SafeLane и создайте пустой репозиторий если его еще нет.

## Шаг 2: Настройте Git Credentials

### Вариант A: Использовать Personal Access Token (рекомендуется)

1. На GitHub перейти в Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Создать новый token с правами `repo` (полный доступ к репозиториям)
3. Скопировать token

### Вариант B: Использовать SSH ключи

```bash
ssh-keygen -t rsa -b 4096 -f ~/.ssh/id_rsa
# Добавить public key на https://github.com/settings/keys
```

## Шаг 3: Выполнить Push

### Если используете HTTPS + Personal Access Token:

```powershell
cd "C:\Я\ICIDS\SafeLane"

# Обновить remote URL для использования HTTPS с authentication
git remote set-url origin https://<YOUR_GITHUB_USERNAME>:<YOUR_TOKEN>@github.com/Strelok14/SafeLane.git

# Выполнить push
git push -u origin main --force
```

### Если используете SSH:

```powershell
cd "C:\Я\ICIDS\SafeLane"

# Изменить remote на SSH URL
git remote set-url origin git@github.com:Strelok14/SafeLane.git

# Выполнить push
git push -u origin main --force
```

## Шаг 4: Проверить результат

После успешного push, репозиторий будет доступен на:
https://github.com/Strelok14/SafeLane

## Текущее состояние репозитория

- **Расположение**: `C:\Я\ICIDS\SafeLane`
- **Ветка**: main
- **Файлы**: SecureFileTransfer.sln + SecureFileTransfer/ (все исходники без build артефактов)
- **Сборка**: ✅ Успешна (11.2 сек)
- **Коммиты**: 1 (feat: SafeLane - Standalone secure LAN file transfer application)

## Что дальше?

После push в GitHub можно:
1. Клонировать репозиторий на другую машину: `git clone https://github.com/Strelok14/SafeLane.git`
2. Работать с кодом и создавать feature ветки
3. Готовить к тестированию: `cd SafeLane && dotnet build SecureFileTransfer.sln`
4. Запустить приложение: `.\SecureFileTransfer\run.bat`
