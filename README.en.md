# EasySave

A file backup application built in C# / .NET 8.0, developed as part of an academic project simulating the full lifecycle of a commercial software product for a fictional publisher (ProSoft).

[🇫🇷 Version française](./README.md)

---

## Stack

- **C# / .NET 8.0** — console app (v1), WPF + MVVM (v2)
- **EasyLog.dll** — custom logging library, fully decoupled from the main application
- **JSON / XML** — selectable log formats; JSON for config and real-time state
- **NuGet** — dependency management
- **GitHub + GitFlow** — versioning and branch management

---

## Features (v1.0)

- Up to 5 configurable backup jobs (full or differential)
- Execution via interactive menu or CLI args (`EasySave.exe 1-3`, `1;3`)
- Per-file JSON log (size, transfer time, paths, status)
- Real-time state file (`state.json`) updated during each backup run
- Bilingual UI — **FR / EN** (switchable from the menu)

## Features (v1.1)

- **XML log format**: choose between JSON and XML from the Settings menu
- **Format persistence**: preference saved in `settings.json` and applied on next launch
- **LoggerFactory**: logger selected automatically at startup based on saved preference
- **EasyLog**: extended with `XmlLogger`, `LogFormat` enum, and `LoggerFactory`

---

## Architecture

```
Easy_Save.sln
├── EasyLog/                Logging library (ILogger, LogEntry, JsonLogger, XmlLogger, LogFormat, LoggerFactory)
├── EasySave.Core/          Shared business logic — reused in v2 without changes
│   ├── Model/Entities/     SaveJob, SaveState, SaveType
│   ├── Model/Service/      SaveExecutor, ConfigService, StateService, AppPaths, LanguageService
│   └── Model/Strategies/   FullSaveStrategy, DifferentialSaveStrategy
├── EasySave.ViewModel/     MVVM ViewModels (SaveJobListViewModel, SaveJobViewModel)
├── EasySave.Console/       Console UI v1 (entry point, menu)
└── EasySave.Tests/         Unit tests (.NET 8.0, MSTest)
```

All runtime files are written to `%AppData%\EasySave\` — no hardcoded paths.

---

## Getting started

```bash
git clone https://github.com/sorooty/Easy_Save
cd Easy_Save
dotnet build
dotnet run --project EasySave.Console
```

**Requirements:** .NET 8.0 SDK — [download](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Branches (GitFlow)

| Branch | Role |
|---|---|
| `main` | Stable released version — merged via tagged PR only |
| `develop` | Shared integration branch — always buildable |
| `feature/name` | Feature development |
| `release/name` | Release preparation |
| `hotfix/name` | Urgent fix on main |

---

## Roadmap

| Version | Description | Status |
|---|---|---|
| v1.0 | Console application | ✅ Released |
| v1.1 | XML log support + format selection | ✅ Released |
| v2.0 | WPF GUI + MVVM architecture | 🔲 Upcoming |
| v3.0 | Advanced features (TBD) | 🔲 Upcoming |

---

## Engineering constraints

- Code, comments and commits in **English** (conventional format: `feat:`, `fix:`, `refactor:`...)
- Zero code duplication — shared logic extracted into services
- Extensible architecture designed for v2/v3 without breaking v1 compatibility
- `EasyLog.dll` must remain backward-compatible across all versions

---

## Context

Academic capstone project — Software Engineering course, CESI A3 FISE INFO 2025-2026.
