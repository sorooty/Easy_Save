# WPF / MVVM

Présentation de l'architecture WPF mise en place à partir de la v2.0.

---

## Projets impliqués

| Projet | Rôle |
|---|---|
| `EasySave.WPF` | Application graphique — Views, Resources, point d'entrée |
| `EasySave.ViewModel` | ViewModels — état UI, commandes, binding |

---

## Structure

```
EasySave.WPF/
├── App.xaml / App.xaml.cs       Point d'entrée, injection des services
├── Views/
│   ├── MainWindow.xaml          Fenêtre principale + navigation par onglets
│   ├── JobListView.xaml         Liste des travaux (lecture + édition inline)
│   └── SettingsView.xaml        Paramètres généraux
└── Resources/
    ├── Strings.resx             Chaînes EN (langue par défaut)
    ├── Strings.fr.resx          Chaînes FR
    ├── Strings.cs               Wrapper statique — propriétés typées pour XAML
    └── Styles.xaml              Thème, couleurs, styles partagés

EasySave.ViewModel/
├── BaseViewModel.cs             INotifyPropertyChanged, SetProperty<T>
├── RelayCommand.cs              ICommand universel
├── MainViewModel.cs             Navigation entre vues
├── SaveJobViewModel.cs          Un ViewModel par travail
├── SaveJobListViewModel.cs      Liste orchestratrice
└── SettingsViewModel.cs         Paramètres
```

---

## BaseViewModel

Fournit `INotifyPropertyChanged` et un helper `SetProperty<T>` :

```csharp
protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
{
    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    field = value;
    OnPropertyChanged(name);
    return true;
}
```

---

## RelayCommand

`ICommand` générique réutilisable dans tous les ViewModels :

```csharp
public class RelayCommand : ICommand
{
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
}
```

---

## SaveJobViewModel

ViewModel par travail. Expose :

- Propriétés de lecture : `Name`, `SourceFolder`, `TargetFolder`, `IsFullType`
- Progression : `Progress` (0–100), `RemainingFiles`, `IsRunning`
- Mode édition : `IsEditing`, `EditName`, `EditSourceFolder`, `EditTargetFolder`
- Commandes : `ExecuteCommand`, `DeleteCommand`, `StartEditCommand`, `ConfirmEditCommand`, `CancelEditCommand`
- Événement : `EditConfirmed` — signale à `SaveJobListViewModel` de persister la config

---

## Localisation

`Strings.cs` expose chaque clé `.resx` comme propriété statique :

```csharp
// Strings.cs
public static string Job_Edit      => ResourceManager.GetString("Job_Edit") ?? "Edit";
public static string Job_Save      => ResourceManager.GetString("Job_Save") ?? "Save";
public static string Job_CancelEdit => ResourceManager.GetString("Job_CancelEdit") ?? "Cancel";
```

En XAML :

```xml
<TextBlock Text="{x:Static res:Strings.Job_Edit}" />
```

Le changement de langue se fait via `CultureInfo.CurrentUICulture` et un redémarrage de la fenêtre principale.

---

## Pattern de progression

```
ISaveStrategy.ExecuteSaveJob(..., IProgress<SaveState>? progress)
    │
    │ après chaque fichier copié
    ▼
progress?.Report(state)
    │
    ▼
SaveJobViewModel.Progress property
    │
    ▼
<ProgressBar Value="{Binding Progress}" />
```

L'interface `IProgress<T>` permet à la stratégie (dans un thread background) de mettre à jour l'UI sur le thread principal via `Progress<T>` — aucun `Dispatcher.Invoke` manuel nécessaire.
