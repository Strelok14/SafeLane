# 🚀 SafeLane - ИНСТРУКЦИЯ ПО PUSH В GITHUB

Репозиторий SafeLane полностью подготовлен и находится в `c:\Я\ICIDS\SafeLane`

**Проблема:** При попытке git push возникает ошибка 403 (Permission denied) из-за несовпадения credentials в Windows Credential Manager.

## ⚠️ Текущая ситуация

```
Error: The requested URL returned error: 403
fatal: unable to access 'https://github.com/Strelok14/SafeLane.git/'
Reason: Permission to Blizzek
```

Windows Credential Manager хранит старые credentials от пользователя "Blizzek", но мы коммитим как "Strelok14".

---

## ✅ РЕШЕНИЕ 1: Использовать GitHub CLI (РЕКОМЕНДУЕТСЯ)

Если у вас установлен GitHub CLI - это самый простой способ:

```powershell
cd "C:\Я\ICIDS\SafeLane"

# 1. Аутентифицироваться в GitHub (если еще не сделано)
gh auth login
# Выбрать: HTTPS, затем авторизоваться через браузер

# 2. Автоматически настроить git
gh auth setup-git

# 3. Выполнить push
git push -u origin main
```

**Результат:** ✅ Репозиторий перенесен в https://github.com/Strelok14/SafeLane

---

## ✅ РЕШЕНИЕ 2: Использовать Personal Access Token

### Шаг 1: Создать Personal Access Token на GitHub

1. Перейти на https://github.com/settings/tokens
2. Нажать "Generate new token" → "Generate new token (classic)"
3. Дать название: "SafeLane Push"
4. Выбрать scope: ☑️ **repo** (полный доступ к репозиториям)
5. Нажать "Generate token"
6. **Скопировать token** (он видимо только один раз!)

### Шаг 2: Выполнить push с token

```powershell
cd "C:\Я\ICIDS\SafeLane"

# Используя PowerShell скрипт (прост):
.\push_to_github.ps1 -Token "ghp_xxxxxxxxxxxxxxxx"

# Или вручную (сложнее):
git remote set-url origin "https://Strelok14:[TOKEN]@github.com/Strelok14/SafeLane.git"
git push -u origin main
```

**Пример токена:** `ghp_1A2B3C4D5E6F7G8H9I0J1K2L3M4N5O6P7Q8R9S`

---

## ✅ РЕШЕНИЕ 3: Использовать SSH ключи

### Шаг 1: Создать SSH ключ (если нет)

```powershell
# Генерировать ключ
ssh-keygen -t rsa -b 4096 -f $env:USERPROFILE\.ssh\id_rsa
# Оставить passphrase пустым (нажать Enter 2 раза)
```

### Шаг 2: Добавить SSH ключ на GitHub

1. Скопировать public key:
```powershell
Get-Content $env:USERPROFILE\.ssh\id_rsa.pub | Set-Clipboard
# Или вручную открыть файл и скопировать содержимое
```

2. Перейти на https://github.com/settings/keys
3. Нажать "New SSH key"
4. Вставить содержимое id_rsa.pub
5. Нажать "Add SSH key"

### Шаг 3: Выполнить push через SSH

```powershell
cd "C:\Я\ICIDS\SafeLane"

# Используя PowerShell скрипт:
.\push_to_github.ps1 -UseSsh $true

# Или вручную:
git remote set-url origin git@github.com:Strelok14/SafeLane.git
git push -u origin main
```

---

## ❌ Что НЕ нужно делать

- ❌ **НЕ** добавлять token/пароль в git.config
- ❌ **НЕ** коммитить файлы с credentials
- ❌ **НЕ** использовать plain-text пароли GitHub

---

## 📋 Чек-лист перед push

```
✅ Репозиторий находится в: C:\Я\ICIDS\SafeLane
✅ Все файлы на месте: SecureFileTransfer.sln, SecureFileTransfer/
✅ Сборка успешна: dotnet build SecureFileTransfer.sln
✅ Git репозиторий инициализирован: .git/ папка существует
✅ Первый коммит создан: git log показывает "feat: SafeLane - ..."
✅ Remote настроен: git remote -v показывает https://github.com/Strelok14/SafeLane.git
✅ Все изменения committed: git status чистый
```

---

## 🔍 Проверка после push

После успешного push проверить репозиторий:

1. Открыть https://github.com/Strelok14/SafeLane
2. Убедиться что видны файлы:
   - README.md
   - SecureFileTransfer.sln
   - SecureFileTransfer/ папка с подпапками Core и App

3. Проверить коммиты:
   ```
   git log --oneline
   # Должен показать: feat: SafeLane - Standalone secure LAN file transfer application
   ```

4. Клонировать на другую машину для проверки:
   ```
   git clone https://github.com/Strelok14/SafeLane.git
   cd SafeLane
   dotnet build SecureFileTransfer.sln
   ```

---

## 📞 Если что-то не работает

**Ошибка: "fatal: unable to access ... 403 Forbidden"**
- Проверить что token/SSH ключ правильный
- Проверить что репозиторий существует на GitHub
- Убедиться что account имеет права на создание репозиториев

**Ошибка: "repository not found"**
- Убедиться что https://github.com/Strelok14/SafeLane существует
- Проверить орфографию в git remote URL

**Ошибка: "Please tell me who you are" (git config user.name)**
- Уже исправлено: `git config user.name "Strelok14"`
- Проверить: `git config user.name`

---

## 🎯 После успешного push

Репозиторий будет доступен и вы сможете:

1. **Работать с ветками:**
   ```
   git branch -b feature/new-feature
   git push -u origin feature/new-feature
   ```

2. **Работать на других машинах:**
   ```
   git clone https://github.com/Strelok14/SafeLane.git
   cd SafeLane
   dotnet build SecureFileTransfer.sln
   ```

3. **Добавлять collaborators:**
   - Settings → Collaborators → Add people

---

**Готово к push! Выберите один из методов выше и выполните его.** 🚀
