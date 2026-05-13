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
    Logiciel de sauvegarde de fichiers — C# · .NET 8 · WPF · MVVM
    <br />
    <a href="https://sorooty.github.io/Easy_Save/"><strong>Documentation »</strong></a>
    <br /><br />
    <a href="https://github.com/sorooty/Easy_Save/issues/new?labels=bug">Signaler un bug</a>
    &middot;
    <a href="https://github.com/sorooty/Easy_Save/issues/new?labels=enhancement">Suggérer une feature</a>
    &middot;
    <a href="README.en.md">English</a>
  </p>
</div>

---

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table des matières</summary>
  <ol>
    <li><a href="#à-propos">À propos</a></li>
    <li><a href="#stack">Stack</a></li>
    <li><a href="#fonctionnalités">Fonctionnalités</a></li>
    <li><a href="#architecture">Architecture</a></li>
    <li><a href="#démarrage-rapide">Démarrage rapide</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#gitflow">GitFlow</a></li>
    <li><a href="#liens">Liens</a></li>
  </ol>
</details>

---

## À propos

EasySave est un logiciel de sauvegarde développé dans le cadre du cours de Génie Logiciel (CESI A3 FISE INFO 2025-2026). Il simule le cycle de vie complet d'un produit commercial chez l'éditeur fictif **ProSoft** : de l'application console minimaliste (v1.0) à l'interface graphique WPF complète (v2.0), jusqu'aux sauvegardes parallèles et la supervision centralisée (v3.0).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Stack

[![CSharp][csharp-shield]][csharp-url]
[![DotNet][dotnet-shield]][dotnet-url]
[![WPF][wpf-shield]][wpf-url]

| Composant | Rôle |
|---|---|
| **C# / .NET 8.0** | Langage et runtime |
| **WPF + MVVM** | Interface graphique (v2.0+) |
| **EasyLog.dll** | Bibliothèque de logging découplée (JSON / XML) |
| **NuGet** | Gestion des dépendances |
| **GitHub + GitFlow** | Versioning et gestion des branches |

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Fonctionnalités

### v1.0 — Console
- Jusqu'à 5 travaux de sauvegarde (complète ou différentielle)
- Exécution via menu interactif ou CLI (`EasySave.exe 1-3`, `1;3`)
- Log JSON journalier par transfert de fichier
- Fichier d'état temps réel (`state.json`)
- Interface bilingue FR / EN

### v1.1 — XML Logger
- Choix du format de log : JSON ou XML
- `LoggerFactory` — sélection automatique au démarrage
- Préférence persistée dans `settings.json`

### v2.0 — WPF / MVVM
- Interface graphique WPF, abandon de la console
- Travaux de sauvegarde illimités
- Cryptage via **[CryptoSoft](https://github.com/sorooty/CryptoSoft)** (XOR, extensions configurables)
- Blocage automatique si un logiciel métier est détecté
- Barre de progression en temps réel par travail
- Édition inline des travaux
- Vue Paramètres généraux (format log, extensions crypto, logiciel métier)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Architecture

```
Easy_Save/
├── EasyLog/                DLL de logging — JsonLogger, XmlLogger, LoggerFactory
├── EasySave.Core/          Logique métier partagée (console + WPF)
│   ├── Model/Entities/     SaveJob · SaveState · SaveType
│   ├── Model/Service/      SaveExecutor · ConfigService · StateService
│   │                       AppPaths · CryptoService · BusinessAppWatcher
│   └── Model/Strategies/   FullSaveStrategy · DifferentialSaveStrategy
├── EasySave.ViewModel/     ViewModels MVVM (v2.0+)
├── EasySave.WPF/           Application graphique WPF (v2.0+)
│   ├── Views/              JobListView · SettingsView · MainWindow
│   └── Resources/          Localisation .resx FR/EN · Styles
└── EasySave.Tests/         Tests unitaires MSTest
```

> Tous les fichiers runtime sont écrits dans `%AppData%\EasySave\` — aucun chemin hardcodé.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Démarrage rapide

**Prérequis :** [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
git clone https://github.com/sorooty/Easy_Save.git
cd Easy_Save
dotnet build
```

**Interface graphique (v2.0) :**
```bash
dotnet run --project EasySave.WPF
```

**Interface console (v1.0 / v1.1) :**
```bash
dotnet run --project EasySave.Console
```

**Tests :**
```bash
dotnet test
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Roadmap

| Version | Description | Statut |
|---|---|---|
| **v1.0** | Application console, 5 travaux, EasyLog.dll | ✅ Livré |
| **v1.1** | Log XML, LoggerFactory | ✅ Livré |
| **v2.0** | WPF/MVVM, CryptoSoft, logiciel métier, progression | ✅ Livré |
| **v3.0** | Sauvegardes parallèles, Play/Pause/Stop, Docker logs | 🔄 En cours |

Voir les [issues ouvertes](https://github.com/sorooty/Easy_Save/issues) pour la liste complète.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## GitFlow

| Branche | Rôle |
|---|---|
| `main` | Version stable — merge via PR taguée uniquement |
| `develop` | Intégration — toujours compilable |
| `feature/*` | Nouvelle fonctionnalité |
| `hotfix/*` | Correction urgente sur main |
| `release/*` | Préparation d'une livraison |

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Liens

- [Documentation complète](https://sorooty.github.io/Easy_Save/)
- [CryptoSoft](https://github.com/sorooty/CryptoSoft) — moteur de cryptage XOR utilisé par EasySave
- Projet — Génie Logiciel · CESI A3 FISE INFO 2025-2026

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
