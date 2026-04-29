# EasySave

A file backup application built in C# / .NET 8.0, developed as part of an academic project simulating the full lifecycle of a commercial software product for a fictional publisher (ProSoft).

[🇫🇷 Version française](./README.md)

---

## Stack

- **C# / .NET 8.0** — console app (v1), WPF + MVVM (v2)
- **EasyLog.dll** — custom logging library, fully decoupled from the main application
- **JSON** — unified format for config, logs and real-time state
- **NuGet** — dependency management
- **GitHub + GitFlow** — versioning and branch management

---

## Features (v1.0)

- Up to 5 configurable backup jobs (full or differential)
- Execution via interactive menu or CLI args (`EasySave.exe 1-3`, `1;3`)
- Per-file JSON daily log (size, transfer time, paths, error flag)
- Real-time state file (`state.json`) updated during each backup run
- Bilingual UI — **FR / EN** (auto-detected from system culture)

---

## Architecture

```
EasySave.slnx
├── EasyLog/                Logging library (ILogger, LogEntry, JsonLogger)
├── EasySave.Core/          Shared business logic — reused in v2 without changes
│   ├── Models/             BackupJob, BackupState, BackupType
│   └── Services/           BackupEngine, ConfigService, StateWriter, AppPaths
└── EasySave.Console/       Console UI v1 (entry point, menu, CLI arg parsing)
    └── Resources/          FR / EN localisation (.resx)
```

All runtime files are written to `%AppData%\EasySave\` — no hardcoded paths.

---

## Getting started

```bash
git clone https://github.com/sorooty/EasySave
cd EasySave
dotnet build
dotnet run --project EasySave.Console
```

**Requirements:** .NET 8.0 SDK — [download](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Branches (GitFlow)

| Branch | Role |
|---|---|
| `master` | Stable released version — merged via tagged PR only |
| `develop` | Shared integration branch — always buildable |
| `feature/name` | Feature development |
| `debug/name` | Bug fix during development |
| `hotfix/name` | Urgent fix on master |

---

## Roadmap

| Version | Description | Status |
|---|---|---|
| v1.0 | Console application | 🔧 In progress |
| v2.0 | WPF GUI + MVVM architecture | 🔲 Upcoming |
| v3.0 | Advanced features (TBD) | 🔲 Upcoming |

---

## Engineering constraints

- Code and filenames in **English**
- Comments in **French**
- Commits in **French** (conventional format: `feat:`, `fix:`, `refactor:`...)
- Zero code duplication — shared logic extracted into services
- Extensible architecture designed for v2/v3 without breaking v1 compatibility
- `EasyLog.dll` must remain backward-compatible across all versions

---

## Context

Academic capstone project — Software Engineering course, CESI A3 FISE INFO 2025-2026.
