# Vue d'ensemble de l'architecture

---

## Structure de la solution

```
Easy_Save/
├── EasyLog/                  Bibliothèque de log (DLL, versionnée séparément)
├── EasySave.Core/            Logique métier partagée entre toutes les interfaces
│   ├── Models/               SaveJob, SaveState, SaveType
│   └── Services/             SaveExecutor, ConfigService, StateService, AppPaths
│                             + CryptoService, BusinessAppWatcher (v2.0+)
├── EasySave.Console/         Application console (v1.0 / v1.1)
├── EasySave.ViewModel/       ViewModels WPF (v2.0+)
├── EasySave.WPF/             Application graphique WPF (v2.0+)
│   ├── Views/                Fenêtres et UserControls XAML
│   └── Resources/            Styles, localisation .resx
└── EasySave.Tests/           Tests unitaires (MSTest)
```

---

## Séparation des responsabilités

```
┌───────────────────────────────┐
│         EasySave.WPF          │  Interface graphique — XAML uniquement
│         Views / Resources     │  Aucune logique métier
└──────────────┬────────────────┘
               │  data binding
┌──────────────▼────────────────┐
│      EasySave.ViewModel       │  Présentation — INotifyPropertyChanged
│      BaseViewModel, SaveJobVM │  ICommand, IProgress, events
└──────────────┬────────────────┘
               │  appels de services
┌──────────────▼────────────────┐
│       EasySave.Core           │  Métier — aucune dépendance UI
│       SaveExecutor, Services  │  Réutilisable (console, WPF, tests)
└──────────────┬────────────────┘
               │  journalisation
┌──────────────▼────────────────┐
│          EasyLog              │  DLL indépendante — JsonLogger, XmlLogger
└───────────────────────────────┘
```

---

## Pattern MVVM

**EasySave v2.0+** suit strictement le pattern **Model–View–ViewModel** :

| Couche | Projet | Rôle |
|---|---|---|
| **Model** | `EasySave.Core` | Logique métier, entités, services |
| **ViewModel** | `EasySave.ViewModel` | État UI, commandes, liaison de données |
| **View** | `EasySave.WPF` | XAML pur, data binding, aucun code métier |

### Règles strictes

- Pas de `new SaveExecutor()` dans une View
- Pas de logique conditionnelle dans le code-behind `.xaml.cs`
- Toute commande UI passe par `RelayCommand` dans un ViewModel
- Tout texte visible passe par les fichiers `.resx`

---

## Flux d'exécution d'une sauvegarde

```
User clicks "Execute"
        │
SaveJobViewModel.ExecuteCommand
        │
SaveExecutor.RunAsync(job, progress)
        │
BusinessAppWatcher.IsRunning() ──→ true? → throw/return false
        │
ISaveStrategy.ExecuteSaveJob(job, state, progress)
        │
  ┌─────┴──────┐
  │ Per file:  │
  │ Copy file  │
  │ CryptoSvc  │ (if extension matches)
  │ Log entry  │
  │ progress.Report(state) ──→ ViewModel.Progress ──→ ProgressBar
  └────────────┘
```

---

## Conventions de nommage

| Élément | Convention | Exemple |
|---|---|---|
| Classes, méthodes, propriétés | PascalCase | `SaveExecutor`, `RunAsync` |
| Variables locales, paramètres | camelCase | `sourceFile`, `jobName` |
| Classes domaine sauvegarde | Préfixe `Save*` | `SaveJob`, `SaveState`, `SaveType` |
| Views WPF | Suffixe `View` | `JobListView`, `SettingsView` |
| ViewModels | Suffixe `ViewModel` | `SaveJobViewModel` |
| Services | Suffixe `Service` ou `Watcher` | `CryptoService`, `BusinessAppWatcher` |

---

## Langues et localisation

- Tout le code, les commentaires et les commits sont en **anglais**
- Toutes les chaînes visibles par l'utilisateur passent par les fichiers `.resx` (FR + EN)
- `Strings.cs` expose les clés comme propriétés statiques typées pour XAML
- Aucun texte hardcodé en XAML ou dans le code-behind
