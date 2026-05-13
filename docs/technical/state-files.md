# Fichiers d'état et logs

---

## `state.json` — État temps réel

Fichier unique mis à jour en temps réel pendant l'exécution de chaque travail.  
**Emplacement :** `%AppData%\EasySave\state.json`

### Structure

```json
[
  {
    "JobName": "backup-docs",
    "IsActive": true,
    "TotalFiles": 42,
    "TotalSizeBytes": 1048576,
    "RemainingFiles": 17,
    "RemainingBytes": 430080,
    "Progress": 59,
    "CurrentSourceFile": "D:\\Documents\\rapport.txt",
    "CurrentTargetFile": "E:\\Backup\\rapport.txt",
    "Timestamp": "2026-05-13T09:42:11Z"
  },
  {
    "JobName": "backup-photos",
    "IsActive": false,
    "TotalFiles": 0,
    "Progress": 0,
    "Timestamp": "2026-05-13T08:00:00Z"
  }
]
```

### Champs

| Champ | Description |
|---|---|
| `JobName` | Nom du travail |
| `IsActive` | `true` si en cours d'exécution |
| `TotalFiles` | Nombre total de fichiers à copier |
| `TotalSizeBytes` | Taille totale en octets |
| `RemainingFiles` | Fichiers restants |
| `RemainingBytes` | Octets restants |
| `Progress` | Pourcentage d'avancement (0–100) |
| `CurrentSourceFile` | Fichier source en cours |
| `CurrentTargetFile` | Fichier destination en cours |
| `Timestamp` | Horodatage de la dernière mise à jour |

---

## Logs journaliers

Un fichier par jour, une entrée par fichier transféré.

**Emplacement :** `%AppData%\EasySave\logs\YYYY-MM-DD.json` (ou `.xml`)

→ [Guide complet des logs](../guide/logs.md)

---

## `StateService`

`StateService` gère la lecture et l'écriture de `state.json` :

```csharp
public interface IStateService
{
    List<SaveState> ReadAll();
    void UpdateState(SaveState state);
}
```

`UpdateState` est appelé par les stratégies après chaque fichier copié — en plus de `progress?.Report(state)` pour l'UI.

---

## Lisibilité dans Notepad

Les fichiers JSON sont écrits avec **retours à la ligne et indentation** pour permettre une lecture directe dans Notepad :

```csharp
JsonSerializer.Serialize(data, new JsonSerializerOptions
{
    WriteIndented = true
});
```

Les fichiers XML sont écrits avec **indentation XML** via `XmlWriterSettings.Indent = true`.
