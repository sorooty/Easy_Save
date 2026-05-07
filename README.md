# EasySave

Logiciel de sauvegarde de fichiers développé en C# / .NET 8.0, dans le cadre d'un projet académique simulant le cycle de vie d'un produit commercial chez un éditeur fictif (ProSoft).

[🇬🇧 English version](./README.en.md)

---

## Stack

- **C# / .NET 8.0** — application console (v1), WPF + MVVM (v2)
- **EasyLog.dll** — bibliothèque de logging maison, découplée de l'application principale
- **JSON** — format universel pour la config, les logs et l'état temps réel
- **NuGet** — gestion des dépendances
- **GitHub + GitFlow** — versioning et gestion des branches

---

## Fonctionnalités (v1.0)

- Création de jusqu'à 5 travaux de sauvegarde (complète ou différentielle)
- Exécution via menu interactif ou arguments CLI (`EasySave.exe 1-3`, `1;3`)
- Log JSON par transfert de fichier (taille, durée, chemins, statut)
- Fichier d'état temps réel (`state.json`) mis à jour pendant chaque sauvegarde
- Interface bilingue **FR / EN** (changement de langue depuis le menu)

---

## Architecture

```
Easy_Save.sln
├── EasyLog/                Bibliothèque de logging (ILogger, LogEntry, JsonLogger)
├── EasySave.Core/          Logique métier partagée — réutilisée en v2 sans modification
│   ├── Model/Entities/     SaveJob, SaveState, SaveType
│   ├── Model/Service/      SaveExecutor, ConfigService, StateService, AppPaths, LanguageService
│   └── Model/Strategies/   FullSaveStrategy, DifferentialSaveStrategy
├── EasySave.ViewModel/     ViewModels MVVM (SaveJobListViewModel, SaveJobViewModel)
├── EasySave.Console/       Interface console v1 (point d'entrée, menu)
└── EasySave.Tests/         Tests unitaires (.NET 8.0, MSTest)
```

Les fichiers de runtime sont écrits dans `%AppData%\EasySave\` (aucun chemin hardcodé).

---

## Lancer le projet

```bash
git clone https://github.com/sorooty/Easy_Save
cd Easy_Save
dotnet build
dotnet run --project EasySave.Console
```

**Prérequis :** .NET 8.0 SDK — [télécharger](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Branches (GitFlow)

| Branche | Rôle |
|---|---|
| `main` | Version stable livrée — merge uniquement via PR taguée |
| `develop` | Branche d'intégration commune — toujours compilable |
| `feature/nom` | Développement d'une fonctionnalité |
| `release/nom` | Préparation d'une livraison |
| `hotfix/nom` | Correction urgente sur main |

---

## Roadmap

| Version | Description | Statut |
|---|---|---|
| v1.0 | Application console | ✅ Livré |
| v1.1 | Support log XML + choix du format | 🔧 En cours |
| v2.0 | Interface graphique WPF + MVVM | 🔲 À venir |
| v3.0 | Fonctionnalités avancées (TBD) | 🔲 À venir |

---

## Contraintes du projet

- Code, commentaires et commits en **anglais** (format conventionnel : `feat:`, `fix:`, `refactor:`...)
- Zéro redondance de code — logique partagée extraite en services
- Architecture extensible conçue pour v2/v3 sans casser la compatibilité v1
- `EasyLog.dll` rétrocompatible entre toutes les versions

---

## Contexte

Projet fil rouge — cours de Génie Logiciel, CESI A3 FISE INFO 2025-2026.
