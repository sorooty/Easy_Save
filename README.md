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
- Log journalier JSON par transfert de fichier (taille, durée, chemins)
- Fichier d'état temps réel (`state.json`) mis à jour pendant chaque sauvegarde
- Interface bilingue **FR / EN** (détection automatique de la langue système)

---

## Architecture

```
EasySave.slnx
├── EasyLog/                Bibliothèque de logging (ILogger, LogEntry, JsonLogger)
├── EasySave.Core/          Logique métier partagée — réutilisée en v2 sans modification
│   ├── Models/             BackupJob, BackupState, BackupType
│   └── Services/           BackupEngine, ConfigService, StateWriter, AppPaths
└── EasySave.Console/       Interface console v1 (point d'entrée, menu, parsing CLI)
    └── Resources/          Localisation FR / EN (.resx)
```

Les fichiers de runtime sont écrits dans `%AppData%\EasySave\` (aucun chemin hardcodé).

---

## Lancer le projet

```bash
git clone https://github.com/sorooty/EasySave
cd EasySave
dotnet build
dotnet run --project EasySave.Console
```

**Prérequis :** .NET 8.0 SDK — [télécharger](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Branches (GitFlow)

| Branche | Rôle |
|---|---|
| `master` | Version stable livrée — merge uniquement via PR taguée |
| `develop` | Branche d'intégration commune — toujours compilable |
| `feature/nom` | Développement d'une fonctionnalité |
| `debug/nom` | Correction de bug en cours de dev |
| `hotfix/nom` | Correction urgente sur master |

---

## Roadmap

| Version | Description | Statut |
|---|---|---|
| v1.0 | Application console | 🔧 En cours |
| v2.0 | Interface graphique WPF + MVVM | 🔲 À venir |
| v3.0 | Fonctionnalités avancées (TBD) | 🔲 À venir |

---

## Contraintes du projet

- Code et noms de fichiers en **anglais**
- Commentaires en **français**
- Commits en **français** (format conventionnel : `feat:`, `fix:`, `refactor:`...)
- Zéro redondance de code
- Architecture extensible conçue pour v2/v3 sans casser la compatibilité v1
- `EasyLog.dll` rétrocompatible entre toutes les versions

---

## Contexte

Projet fil rouge — cours de Génie Logiciel, CESI A3 FISE INFO 2025-2026.
