# Core — Logique métier

`EasySave.Core` est le cœur du projet. Il est partagé entre l'interface console (v1.0/v1.1) et l'interface WPF (v2.0+). Il ne dépend d'aucune technologie d'interface.

---

## Models

### `SaveJob`

Définit un travail de sauvegarde.

```csharp
public class SaveJob
{
    public string Name          { get; set; }
    public string SourceFolder  { get; set; }
    public string TargetFolder  { get; set; }
    public SaveType Type        { get; set; }
}
```

### `SaveType`

```csharp
public enum SaveType { Full, Differential }
```

### `SaveState`

État d'avancement d'un travail, écrit dans `state.json` et propagé à l'UI via `IProgress<SaveState>`.

```csharp
public class SaveState
{
    public string   JobName           { get; set; }
    public bool     IsActive          { get; set; }
    public int      TotalFiles        { get; set; }
    public long     TotalSizeBytes    { get; set; }
    public int      RemainingFiles    { get; set; }
    public long     RemainingBytes    { get; set; }
    public int      Progress          { get; set; }  // 0–100
    public string   CurrentSourceFile { get; set; }
    public string   CurrentTargetFile { get; set; }
    public DateTime Timestamp         { get; set; }
}
```

---

## Services

### `SaveExecutor`

Orchestration principale. Lance la stratégie adaptée au type de travail, surveille le logiciel métier, propage la progression.

```csharp
public async Task<bool> RunAsync(SaveJob job, IProgress<SaveState>? progress = null)
```

- Appelle `BusinessAppWatcher.IsRunning()` avant chaque fichier
- Passe `progress` à la stratégie pour un rapport par fichier

### `ConfigService`

Lecture et écriture de la configuration :
- `jobs.json` — liste des travaux
- `settings.json` — format de log, extensions crypto, chemin CryptoSoft, logiciel métier

### `StateService`

Lecture et écriture de `state.json`. Mis à jour par les stratégies à chaque fichier traité.

### `AppPaths`

Centralise tous les chemins de fichiers. Garantit que rien ne pointe vers `C:\temp\` ou un chemin absolu hardcodé.

```csharp
AppPaths.JobsFile     // %AppData%\EasySave\jobs.json
AppPaths.StateFile    // %AppData%\EasySave\state.json
AppPaths.LogsFolder   // %AppData%\EasySave\logs\
AppPaths.SettingsFile // %AppData%\EasySave\settings.json
```

### `CryptoService` *(v2.0+)*

Lance `CryptoSoft.exe` en sous-processus sur un fichier et retourne le temps d'exécution en ms (ou un code d'erreur négatif).

### `BusinessAppWatcher` *(v2.0+)*

Vérifie si le processus configuré est en cours d'exécution.

```csharp
public bool IsRunning()  // true = logiciel métier actif
```

---

## Stratégies de sauvegarde

Le pattern **Strategy** est utilisé pour isoler les algorithmes de copie.

```
ISaveStrategy
├── FullSaveStrategy         Copie tous les fichiers
└── DifferentialSaveStrategy Copie uniquement les fichiers absents ou modifiés
```

Signature de l'interface :

```csharp
public interface ISaveStrategy
{
    Task ExecuteSaveJob(
        SaveJob job,
        SaveState state,
        IStateService stateService,
        IEasyLogger logger,
        CryptoService? cryptoService,
        BusinessAppWatcher? businessWatcher,
        IProgress<SaveState>? progress = null);
}
```

`progress?.Report(state)` est appelé **après chaque fichier copié**, permettant à l'UI de se mettre à jour en temps réel.
