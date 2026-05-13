# EasySave — Logiciel de sauvegarde ProSoft

**EasySave** est un logiciel de sauvegarde développé par l'équipe ProSoft dans le cadre du projet génie logiciel PGE A3 FISE INFO 2025–2026. Il permet de créer, configurer et exécuter des travaux de sauvegarde (complète ou différentielle) depuis une interface graphique moderne.

---

## Présentation

| | |
|---|---|
| **Éditeur** | ProSoft |
| **Langage** | C# / .NET 8.0 |
| **Interface** | WPF (v2.0+) · Console (v1.0 / v1.1) |
| **Architecture** | MVVM (v2.0+) |
| **Plateforme** | Windows |
| **Dépôt** | [github.com/sorooty/Easy_Save](https://github.com/sorooty/Easy_Save) |

---

## Versions

| Version | Statut | Interface | Points clés |
|---|---|---|---|
| [v1.0](versions/v1.0.md) | ✅ Livré | Console | 5 travaux max, JSON logs, EasyLog.dll |
| [v1.1](versions/v1.1.md) | ✅ Livré | Console | Format XML optionnel, LoggerFactory |
| [v2.0](versions/v2.0.md) | ✅ Livré | WPF / MVVM | Travaux illimités, CryptoSoft, logiciel métier, édition inline |
| [v3.0](versions/v3.0.md) | 🔄 En cours | WPF / MVVM | Parallèle, Play/Pause/Stop, priorités, Docker logs |

---

## Architecture en bref

```
Easy_Save/
├── EasyLog/            Bibliothèque de log (DLL)
├── EasySave.Core/      Logique métier partagée
├── EasySave.Console/   Interface console (v1.0 / v1.1)
├── EasySave.ViewModel/ ViewModels WPF (v2.0+)
├── EasySave.WPF/       Application graphique (v2.0+)
└── EasySave.Tests/     Tests unitaires
```

→ [Architecture détaillée](architecture/overview.md)

---

## Navigation rapide

- **Nouveau sur EasySave ?** → [Guide d'installation](guide/installation.md)
- **Première sauvegarde** → [Premiers pas](guide/quickstart.md)
- **Comprendre le code** → [Architecture](architecture/overview.md)
- **Historique des changements** → [Changelog](changelog.md)
