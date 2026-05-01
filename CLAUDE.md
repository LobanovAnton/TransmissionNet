# TransmissionNet

.NET MAUI клиент для Transmission torrent daemon.

## Build & Run

```bash
# Restore и build
dotnet build TransmissionNet/TransmissionNet.csproj

# Запуск (iOS simulator)
dotnet build TransmissionNet/TransmissionNet.csproj -t:Run -f net10.0-ios

# Publish для iOS (iPhone + iPad)
dotnet publish TransmissionNet/TransmissionNet.csproj -f net10.0-ios -c Release -r ios-arm64 -p:ArchiveOnBuild=true
```

## Зависимости

- .NET 10 (`global.json`)
- CommunityToolkit.Maui
- Transmission.API.RPC.NET (локальная библиотека `../Transmission.API.RPC.NET/`)

## Архитектура

### MVVM

- **Base**: `BindableObject` (MAUI built-in), НЕ сторонние MVVM фреймворки
- **ViewModel hierarchy**:
  - `ShellPageViewModel` — IsRunning, Initialize/Deinitialize lifecycle
  - `ProviderShellPageViewModel` — создаёт ITorrentProvider из настроек
  - `UpdatableShellPageViewModel` — таймер авто-обновления
- **Properties**: стандартный get/set + `OnPropertyChanged()` без аргументов
- **Commands**: Command pattern, не RelayCommand

### Provider Pattern

- `ITorrentProvider` — интерфейс для торрент-клиента
- `TransmissionTorrentProvider` — реализация Transmission RPC
- `TorrentProviderFactory` — фабрика провайдеров

### RPC Field Selection

- `TorrentField` / `SessionField` — enums для выбора полей
- `Dictionary<Enum, string> FieldMap` — маппинг enum → RPC field name
- Каждый caller объявляет `static readonly` массив только нужных полей

### Models

- `SessionSettingsModel` (nullable поля) — для Set-запросов, отправляет только изменённые
- `SessionModel` (non-nullable) — для Get-запросов
- `TorrentSettingsModel` (nullable) — для изменения настроек торрента
- Non-nullable поля без запрошенных данных — ответственность программиста

## Структура проекта

```
TransmissionNet/
├── Controls/        # Custom controls
├── Converters/      # Value converters
├── Extensions/      # Extension methods
├── MainApp/         # App, WindowCreator
├── Platforms/iOS/   # iOS-specific (Info.plist)
├── Services/        # App services
├── Settings/        # Settings pages и ViewModels
├── Shell/           # Shell navigation, base ViewModels
├── TorrentProviders/# ITorrentProvider, models, enums
├── Torrents/        # Torrent pages, ViewModels, PieceMapView
├── Triggers/        # XAML triggers
└── Utils/           # MedianFilter и утилиты
```

## Conventions

- Enums вместо множества bool-свойств
- Один `SetTorrentSettingsAsync` вместо отдельных методов
- Transmission RPC: пустой массив `[]` означает "все файлы"
- EndPiece — exclusive в Transmission API
- Pieces bitfield: big-endian, `bitIndex = 7 - (pieceIndex & 7)`
- `& 7` вместо `% 8` для bit-операций

## MAUI Gotchas

- `x:Reference` в DataTemplate не работает надёжно — использовать code-behind
- `StaticResource` binding не обновляется по PropertyChanged — code-behind для динамических данных
- `CollectionView` виртуализирует views — HeightRequest сохраняется при recycling
- `Switch` не имеет Command/CommandParameter — использовать Toggled event в code-behind
