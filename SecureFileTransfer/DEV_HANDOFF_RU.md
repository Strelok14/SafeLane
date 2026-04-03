# SafeLane / SecureFileTransfer — файл передачи разработки

Этот файл нужен для продолжения работы только с папкой SecureFileTransfer в отдельной рабочей области VS Code.

## Что можно открыть как отдельный проект

Можно работать только с этой папкой:

SecureFileTransfer

Внутри нее уже есть все исходники приложения:

- SecureFileTransfer.App
- SecureFileTransfer.Core
- README.md
- run.bat
- run.sh
- .gitignore

Важно:

- Файла решения SecureFileTransfer.sln внутри этой папки нет, он лежит уровнем выше в корне репозитория SafeLane.
- Поэтому если вы открываете только эту папку в новой рабочей области, собирайте проект не через .sln, а через .csproj.

## Как правильно начать работу в новой рабочей области

### Вариант 1. Лучший

1. Склонировать весь репозиторий SafeLane.
2. Открыть в VS Code только папку SecureFileTransfer.
3. Работать из нее как из отдельного проекта.

### Вариант 2. Можно вручную

1. Скопировать папку SecureFileTransfer в новое место.
2. Открыть именно эту папку в VS Code.
3. Оставить имя папки SecureFileTransfer без переименования, чтобы run.bat и run.sh работали без правок.

## Команды для работы именно из этой папки

Если терминал открыт в папке SecureFileTransfer:

### Сборка Core

```powershell
dotnet build .\SecureFileTransfer.Core\SecureFileTransfer.Core.csproj
```

### Сборка приложения

```powershell
dotnet build .\SecureFileTransfer.App\SecureFileTransfer.App.csproj
```

### Запуск приложения напрямую

```powershell
dotnet run --project .\SecureFileTransfer.App\SecureFileTransfer.App.csproj
```

### Запуск через bat-скрипт

```powershell
.\run.bat
```

### Запуск через shell-скрипт

```bash
chmod +x ./run.sh
./run.sh
```

## Почему dotnet build SecureFileTransfer.sln может падать

Если вы находитесь внутри папки SecureFileTransfer, команда

```powershell
dotnet build SecureFileTransfer.sln
```

не сработает, потому что файла решения в этой папке нет.

Правильно:

- либо открыть корень репозитория SafeLane и собирать через SecureFileTransfer.sln
- либо, если открыта только эта папка, собирать через csproj-файлы, как указано выше

## Что уже реализовано

Проект уже содержит рабочую реализацию безопасной передачи файлов по LAN.

### Основная функциональность

- Windows Forms UI
- встроенный HTTP-сервер на ASP.NET Core / Kestrel
- обмен файлами чанками
- UDP multicast discovery узлов в сети
- список доверенных узлов
- HMAC-SHA256 подпись каждого пакета
- PBKDF2 derivation ключа из общего секрета
- защита от replay-атак
- rate limiting по IP
- сборка файла из чанков на приемной стороне

### Что было дополнительно сделано

- проект вынесен в отдельный репозиторий SafeLane
- удалены лишние build-артефакты и локальные файлы
- интерфейс переведен на русский язык
- добавлена темная тема
- улучшены тексты статусов и сообщений об ошибках

## Текущий стек

- .NET 8
- C#
- Windows Forms
- ASP.NET Core Kestrel
- System.Security.Cryptography
- UDP multicast

## Архитектура проекта

### SecureFileTransfer.Core

Библиотека с бизнес-логикой и безопасностью.

#### Models

Основные DTO и модели:

- UploadChunkRequest.cs
- HandshakeRequest.cs
- HandshakeResponse.cs
- NodeAnnouncement.cs
- TrustedNodeConfig.cs

#### Security

Криптография и канонизация:

- MessageCanonicalizer.cs
- IKeyDerivationService.cs
- KeyDerivationService.cs
- IHmacSignatureService.cs
- HmacSignatureService.cs

#### Protection

Защита протокола:

- IReplayProtectionService.cs
- ReplayProtectionService.cs
- IRateLimiter.cs
- FixedWindowRateLimiter.cs

#### Transfer

Работа с передачей файлов:

- ChunkProcessResult.cs
- IFileChunkAssembler.cs
- FileChunkAssembler.cs

### SecureFileTransfer.App

Приложение с UI, сервером и клиентской логикой.

#### Infrastructure

- AppSettings.cs
- SettingsStore.cs
- TrustedNodeStore.cs
- SequenceGenerator.cs
- ThemeManager.cs

#### Networking

- NodeDiscoveryService.cs
- LocalServerHost.cs
- FileSenderService.cs
- RateLimitingMiddleware.cs

#### Dialogs

- AddNodeForm.cs
- SettingsForm.cs

#### UI

- Form1.cs
- Form1.Designer.cs
- Program.cs

#### ViewModels

- NodeViewModel.cs

## Самые важные файлы для продолжения разработки

Если нужно быстро войти в проект, начинайте отсюда:

### 1. Главная форма

Form1.cs

Здесь находится:

- запуск приложения
- запуск локального сервера
- запуск discovery
- отправка файла
- обновление UI
- работа со статусами и логами

### 2. Разметка формы

Form1.Designer.cs

Здесь находятся:

- кнопки
- таблица узлов
- прогресс-бары
- список логов
- базовая компоновка интерфейса

### 3. Локальный HTTP-сервер

Networking/LocalServerHost.cs

Здесь находятся:

- POST /api/handshake
- POST /api/upload
- GET /health
- проверка подписи
- replay protection
- обработка чанков

### 4. Отправка файла

Networking/FileSenderService.cs

Здесь находятся:

- handshake перед передачей
- разбивка файла на чанки
- подписание пакетов
- POST на удаленный узел
- retry-логика

### 5. Discovery узлов

Networking/NodeDiscoveryService.cs

Здесь находятся:

- UDP multicast announce
- обнаружение других узлов
- обработка входящих анонсов

### 6. Темная тема

Infrastructure/ThemeManager.cs

Здесь находятся:

- цвета темной темы
- применение темы ко всем контролам
- логика стилизации формы

## Что именно было сделано по UI

### Русификация

Переведены:

- кнопки
- статусы
- ошибки
- сообщения в логах
- заголовок окна
- диалоги выбора файла

### Темная тема

Добавлен ThemeManager с цветами:

- фон: темно-серый
- светлый текст
- затемненные контролы
- синий акцент

Тема применяется при загрузке Form1.

## Конфигурация

Настройки создаются и читаются через:

- Infrastructure/SettingsStore.cs
- Infrastructure/TrustedNodeStore.cs

Во время запуска данные хранятся в App_Data.

Основные параметры:

- NodeId
- NodeName
- ListenPort
- DiscoveryPort
- SharedSecret
- ChunkSizeKb
- MaxRequestsPerSecond
- ReceivedDirectory

## Сетевые параметры по умолчанию

- HTTP server: 5077
- UDP discovery: 8888
- multicast group: 239.0.0.1:8888
- chunk size: 64 KB
- rate limit: 10 запросов/сек на IP

## Безопасность

Реализовано:

- HMAC-SHA256 для handshake и чанков
- PBKDF2 для получения симметричного ключа
- FixedTimeEquals для безопасного сравнения подписи
- replay protection через timestamp + sequence number
- rate limiting на входящие запросы

Ограничения текущей версии:

- общий секрет может быть общим для всех узлов
- replay state хранится в памяти и очищается после рестарта
- TLS не используется, так как проект рассчитан на доверенную локальную сеть

## Что проверить первым делом после открытия новой рабочей области

1. Что установлен .NET 8 SDK.
2. Что терминал открыт в папке SecureFileTransfer.
3. Что собирается SecureFileTransfer.App.csproj.
4. Что приложение стартует через dotnet run или run.bat.
5. Что создаются настройки и папка App_Data.

## Быстрый сценарий проверки работоспособности

1. Запустить приложение на двух машинах в одной LAN.
2. На обеих сторонах указать одинаковый Shared Secret.
3. Добавить узлы друг другу в доверенные.
4. Проверить, что /health доступен.
5. Выбрать файл и отправить его.
6. Проверить, что файл появился в ReceivedDirectory.

## Где чаще всего продолжать разработку

Обычно следующий этап изменений будет в одном из направлений:

### UI и UX

Править:

- Form1.cs
- Form1.Designer.cs
- ThemeManager.cs
- Dialogs/*.cs

### Протокол и безопасность

Править:

- Security/*.cs
- Protection/*.cs
- Networking/LocalServerHost.cs
- Networking/FileSenderService.cs

### Обнаружение узлов

Править:

- Networking/NodeDiscoveryService.cs

### Конфигурация и хранение

Править:

- Infrastructure/AppSettings.cs
- Infrastructure/SettingsStore.cs
- Infrastructure/TrustedNodeStore.cs

## Полезные замечания

- Если работаете только с этой папкой, .sln не нужен.
- Для разработки достаточно двух csproj-файлов.
- Если понадобится полная сборка через solution, откройте корень репозитория SafeLane, а не только папку приложения.
- Если будете переносить папку отдельно, не удаляйте run.bat, run.sh и README.md.

## Минимальный набор файлов, который нельзя потерять

- SecureFileTransfer.App
- SecureFileTransfer.Core
- run.bat
- run.sh
- README.md
- .gitignore
- этот файл DEV_HANDOFF_RU.md

## Рекомендуемый порядок входа в разработку

1. Прочитать этот файл.
2. Собрать SecureFileTransfer.App.csproj.
3. Запустить приложение.
4. Открыть Form1.cs.
5. Открыть LocalServerHost.cs.
6. Открыть FileSenderService.cs.
7. Открыть ThemeManager.cs.

## Итог

Да, эту папку можно использовать как отдельную рабочую область для дальнейшей разработки.

Главное правило:

- не собирать через SecureFileTransfer.sln из этой папки
- собирать и запускать через SecureFileTransfer.App.csproj

Если откроете именно папку SecureFileTransfer в VS Code, этого достаточно для продолжения работы.