<a id="readme-top"></a>

<!-- SHIELDS -->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stars][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]

<!-- HEADER -->
<br />
<div align="center">

<h1 align="center">EasySave</h1>

  <p align="center">
    File backup software — C# · .NET 8 · WPF · MVVM
    <br />
    <a href="https://sorooty.github.io/Easy_Save/"><strong>Documentation »</strong></a>
    <br /><br />
    <a href="https://github.com/sorooty/Easy_Save/issues/new?labels=bug">Report a bug</a>
    &middot;
    <a href="https://github.com/sorooty/Easy_Save/issues/new?labels=enhancement">Request a feature</a>
    &middot;
    <a href="README.md">Français</a>
  </p>
</div>

---

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about">About</a></li>
    <li><a href="#stack">Stack</a></li>
    <li><a href="#features">Features</a></li>
    <li><a href="#architecture">Architecture</a></li>
    <li><a href="#getting-started">Getting Started</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#gitflow">GitFlow</a></li>
    <li><a href="#links">Links</a></li>
  </ol>
</details>

---

## About

EasySave is a file backup application developed as part of a Software Engineering course (CESI A3 FISE INFO 2025-2026). It simulates the full lifecycle of a commercial product for the fictional publisher **ProSoft**: from a minimal console app (v1.0) to a full WPF graphical interface (v2.0), and eventually parallel backups with centralized monitoring (v3.0).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Stack

[![CSharp][csharp-shield]][csharp-url]
[![DotNet][dotnet-shield]][dotnet-url]
[![WPF][wpf-shield]][wpf-url]

| Component | Role |
|---|---|
| **C# / .NET 8.0** | Language and runtime |
| **WPF + MVVM** | Graphical interface (v2.0+) |
| **EasyLog.dll** | Decoupled logging library (JSON / XML) |
| **NuGet** | Dependency management |
| **GitHub + GitFlow** | Versioning and branch management |

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Features

### v1.0 — Console
- Up to 5 backup jobs (full or differential)
- Execution via interactive menu or CLI args (`EasySave.exe 1-3`, `1;3`)
- Per-file JSON daily log
- Real-time state file (`state.json`)
- Bilingual UI — FR / EN

### v1.1 — XML Logger
- Selectable log format: JSON or XML
- `LoggerFactory` — automatic selection at startup
- Preference persisted in `settings.json`

### v2.0 — WPF / MVVM
- WPF graphical interface, console abandoned
- Unlimited backup jobs
- Encryption via **[CryptoSoft](https://github.com/sorooty/CryptoSoft)** (XOR, configurable extensions)
- Automatic blocking when a business application is detected
- Real-time progress bar per job
- Inline job editing
- General Settings view (log format, crypto extensions, business app)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Architecture

```
Easy_Save/
├── EasyLog/                Logging DLL — JsonLogger, XmlLogger, LoggerFactory
├── EasySave.Core/          Shared business logic (console + WPF)
│   ├── Model/Entities/     SaveJob · SaveState · SaveType
│   ├── Model/Service/      SaveExecutor · ConfigService · StateService
│   │                       AppPaths · CryptoService · BusinessAppWatcher
│   └── Model/Strategies/   FullSaveStrategy · DifferentialSaveStrategy
├── EasySave.ViewModel/     MVVM ViewModels (v2.0+)
├── EasySave.WPF/           WPF application (v2.0+)
│   ├── Views/              JobListView · SettingsView · MainWindow
│   └── Resources/          FR/EN .resx localization · Styles
└── EasySave.Tests/         MSTest unit tests
```

> All runtime files are written to `%AppData%\EasySave\` — no hardcoded paths.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Getting Started

**Requirements:** [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
git clone https://github.com/sorooty/Easy_Save.git
cd Easy_Save
dotnet build
```

**Graphical interface (v2.0) :**
```bash
dotnet run --project EasySave.WPF
```

**Console interface (v1.0 / v1.1) :**
```bash
dotnet run --project EasySave.Console
```

**Tests:**
```bash
dotnet test
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Roadmap

| Version | Description | Status |
|---|---|---|
| **v1.0** | Console app, 5 jobs, EasyLog.dll | ✅ Released |
| **v1.1** | XML log, LoggerFactory | ✅ Released |
| **v2.0** | WPF/MVVM, CryptoSoft, business app blocker, progress | ✅ Released |
| **v3.0** | Parallel backups, Play/Pause/Stop, Docker logs | 🔄 In progress |

See [open issues](https://github.com/sorooty/Easy_Save/issues) for the full list.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## GitFlow

| Branch | Role |
|---|---|
| `main` | Stable release — merged via tagged PR only |
| `develop` | Integration branch — always buildable |
| `feature/*` | New feature development |
| `hotfix/*` | Urgent fix on main |
| `release/*` | Release preparation |

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Links

- [Full documentation](https://sorooty.github.io/Easy_Save/)
- [CryptoSoft](https://github.com/sorooty/CryptoSoft) — XOR encryption engine used by EasySave
- Project — Software Engineering · CESI A3 FISE INFO 2025-2026

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

<!-- MARKDOWN LINKS -->
[contributors-shield]: https://img.shields.io/github/contributors/sorooty/Easy_Save.svg?style=for-the-badge&color=3f51b5
[contributors-url]: https://github.com/sorooty/Easy_Save/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/sorooty/Easy_Save.svg?style=for-the-badge&color=3f51b5
[forks-url]: https://github.com/sorooty/Easy_Save/network/members
[stars-shield]: https://img.shields.io/github/stars/sorooty/Easy_Save.svg?style=for-the-badge&color=3f51b5
[stars-url]: https://github.com/sorooty/Easy_Save/stargazers
[issues-shield]: https://img.shields.io/github/issues/sorooty/Easy_Save.svg?style=for-the-badge&color=3f51b5
[issues-url]: https://github.com/sorooty/Easy_Save/issues
[csharp-shield]: https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white
[csharp-url]: https://learn.microsoft.com/dotnet/csharp/
[dotnet-shield]: https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[dotnet-url]: https://dotnet.microsoft.com/
[wpf-shield]: https://img.shields.io/badge/WPF-0078D4?style=for-the-badge&logo=windows&logoColor=white
[wpf-url]: https://learn.microsoft.com/dotnet/desktop/wpf/
