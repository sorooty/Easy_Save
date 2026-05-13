# Configuration

---

## `AppPaths` — Centralisation des chemins

Tous les chemins de fichiers passent par `AppPaths`. Aucun chemin absolu (`C:\temp\`, etc.) n'est hardcodé ailleurs dans le code.

```csharp
// EasySave.Core/Services/AppPaths.cs
public static class AppPaths
{
    private static readonly string Base =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");

    public static string JobsFile    => Path.Combine(Base, "jobs.json");
    public static string SettingsFile => Path.Combine(Base, "settings.json");
    public static string StateFile   => Path.Combine(Base, "state.json");
    public static string LogsFolder  => Path.Combine(Base, "logs");
}
```

**Chemin de base :** `%AppData%\EasySave\` = `C:\Users\<nom>\AppData\Roaming\EasySave\`

---

## `jobs.json` — Travaux configurés

```json
[
  {
    "Name": "backup-docs",
    "SourceFolder": "D:\\Documents",
    "TargetFolder": "E:\\Backup\\documents",
    "Type": "Full"
  },
  {
    "Name": "backup-photos",
    "SourceFolder": "D:\\Photos",
    "TargetFolder": "E:\\Backup\\photos",
    "Type": "Differential"
  }
]
```

Géré par `ConfigService.LoadJobs()` / `ConfigService.SaveJobs(jobs)`.

---

## `settings.json` — Paramètres généraux

```json
{
  "LogFormat": "Json",
  "CryptoExtensions": [".txt", ".docx", ".pdf"],
  "CryptoSoftPath": "C:\\Outils\\CryptoSoft\\CryptoSoft.exe",
  "BusinessAppName": "calc.exe"
}
```

| Champ | Type | Description |
|---|---|---|
| `LogFormat` | `"Json"` ou `"Xml"` | Format des fichiers log |
| `CryptoExtensions` | `string[]` | Extensions de fichiers à crypter |
| `CryptoSoftPath` | `string` | Chemin absolu vers `CryptoSoft.exe` |
| `BusinessAppName` | `string` | Nom du processus logiciel métier |

---

## `ConfigService`

```csharp
public class ConfigService
{
    public List<SaveJob> LoadJobs();
    public void SaveJobs(List<SaveJob> jobs);
    public AppSettings LoadSettings();
    public void SaveSettings(AppSettings settings);
}
```

Toutes les lectures/écritures sont en JSON indenté pour la lisibilité dans Notepad.
