![Banner EasySave](assets/banner.png)

# EasySave - Logiciel de sauvegarde ProSoft

**EasySave** est un logiciel de sauvegarde développé par l'équipe ProSoft dans le cadre du projet génie logiciel PGE A3 FISE INFO 2025-2026. Il permet de créer, configurer et exécuter des travaux de sauvegarde (complète ou différentielle) depuis une interface graphique moderne.

---

## Présentation

| | |
|---|---|
| **Editeur** | ProSoft |
| **Langage** | C# / .NET 8.0 |
| **Interface** | WPF (v2.0+) / Console (v1.0 / v1.1) |
| **Architecture** | MVVM (v2.0+) |
| **Plateforme** | Windows |
| **Depot** | [github.com/sorooty/Easy_Save](https://github.com/sorooty/Easy_Save) |

---

## Versions

| Version | Statut | Interface | Points cles |
|---|---|---|---|
| [v1.0](versions/v1.0.md) | Livre | Console | 5 travaux max, JSON logs, EasyLog.dll |
| [v1.1](versions/v1.1.md) | Livre | Console | Format XML optionnel, LoggerFactory |
| [v2.0](versions/v2.0.md) | Livre | WPF / MVVM | Travaux illimites, CryptoSoft, logiciel metier, edition inline |
| [v3.0](versions/v3.0.md) | Livrable final | WPF / MVVM | Parallele, Play/Pause/Stop, priorites, Docker logs |

---

## Architecture en bref

```
Easy_Save/
├── EasyLog/            Bibliotheque de log (DLL)
├── EasySave.Core/      Logique metier partagee
├── EasySave.Console/   Interface console (v1.0 / v1.1)
├── EasySave.ViewModel/ ViewModels WPF (v2.0+)
├── EasySave.WPF/       Application graphique (v2.0+)
└── EasySave.Tests/     Tests unitaires
```

- [Architecture detaillee](architecture/overview.md)

---

## Navigation rapide

- **Nouveau sur EasySave ?** - [Guide d'installation](guide/installation.md)
- **Premiere sauvegarde** - [Premiers pas](guide/quickstart.md)
- **Comprendre le code** - [Architecture](architecture/overview.md)
- **Historique des changements** - [Changelog](changelog.md)
