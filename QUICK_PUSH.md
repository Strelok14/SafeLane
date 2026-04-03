# БЫСТРЫЙ СТАРТ - PUSH SAFELANE В GITHUB

## 🚀 Вариант 1: GitHub CLI (САМЫЙ ПРОСТОЙ)

```powershell
gh auth login
# Выбрать HTTPS, авторизоваться через браузер

gh auth setup-git

cd "c:\Я\ICIDS\SafeLane"
git push -u origin main
```

✅ **Результат:** Готово!

---

## 🚀 Вариант 2: Personal Access Token (БЫСТРО)

### Создать Token:
1. https://github.com/settings/tokens
2. Нажать "Generate new token (classic)"
3. Scope: ☑️ repo
4. Скопировать token

### Push:
```powershell
cd "c:\Я\ICIDS\SafeLane"

# С PowerShell скриптом:
.\push_to_github.ps1 -Token "ghp_1234567890abcdef"

# Или вручную:
git remote set-url origin "https://Strelok14:ghp_1234567890abcdef@github.com/Strelok14/SafeLane.git"
git push -u origin main
```

✅ **Результат:** Готово!

---

## 🚀 Вариант 3: SSH (БЕЗОПАСНО)

### Создать ключ:
```powershell
ssh-keygen -t rsa -b 4096
# Оставить passphrase пустым
```

### Добавить на GitHub:
1. https://github.com/settings/keys → "New SSH key"
2. Скопировать содержимое `$env:USERPROFILE\.ssh\id_rsa.pub`
3. Вставить в GitHub

### Push:
```powershell
cd "c:\Я\ICIDS\SafeLane"

# С PowerShell скриптом:
.\push_to_github.ps1 -UseSsh $true

# Или вручную:
git remote set-url origin git@github.com:Strelok14/SafeLane.git
git push -u origin main
```

✅ **Результат:** Готово!

---

## ✅ ПРОВЕРИТЬ РЕЗУЛЬТАТ

Открыть: https://github.com/Strelok14/SafeLane

Должны быть видны все файлы и структура готова для работы! 🎉

---

## 📖 ПОДРОБНЕЕ

Полные инструкции и решение проблем в [GITHUB_PUSH_GUIDE.md](GITHUB_PUSH_GUIDE.md)
