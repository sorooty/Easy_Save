# EasyLog.dll

`EasyLog` est une **bibliothèque de log indépendante** développée comme DLL séparée. Elle est versionnée dans son propre projet dans la solution et doit rester rétrocompatible avec la v1.0 à chaque évolution.

---

## Contrat de rétrocompatibilité

> **Toutes les évolutions d'EasyLog.dll doivent rester compatibles avec la v1.0.**

Concrètement :
- `JsonLogger` et `LogEntry` ne changent jamais de signature
- Les nouvelles classes (`XmlLogger`, `LoggerFactory`) sont additives
- Aucune interface existante n'est modifiée

---

## Interface

```csharp
public interface IEasyLogger
{
    void Log(LogEntry entry);
}
```

---

## `LogEntry` — entrée de log

```csharp
public class LogEntry
{
    public DateTime Timestamp          { get; set; }
    public string   JobName            { get; set; }
    public string   SourceFile         { get; set; }
    public string   TargetFile         { get; set; }
    public long     FileSizeBytes      { get; set; }
    public long     TransferDurationMs { get; set; }  // négatif = erreur
    public long     EncryptionTimeMs   { get; set; }  // 0 = pas de cryptage (v2.0+)
}
```

---

## Implémentations

| Classe | Format | Disponible depuis |
|---|---|---|
| `JsonLogger` | JSON avec retours à la ligne | v1.0 |
| `XmlLogger` | XML indenté | v1.1 |

### Nommage des fichiers log

```
%AppData%\EasySave\logs\YYYY-MM-DD.json   (JSON)
%AppData%\EasySave\logs\YYYY-MM-DD.xml    (XML)
```

---

## `LoggerFactory` *(v1.1+)*

```csharp
IEasyLogger logger = LoggerFactory.Create(LogFormat.Json);  // ou LogFormat.Xml
```

Au démarrage, l'application lit `settings.json` et crée le logger correspondant. Le même logger est utilisé pour toute la session — changer le format dans les Paramètres prend effet au prochain lancement.

---

## Exemple d'entrée JSON

```json
{
  "Timestamp": "2026-05-13T09:42:11Z",
  "JobName": "backup-docs",
  "SourceFile": "D:\\Documents\\rapport.txt",
  "TargetFile": "E:\\Backup\\rapport.txt",
  "FileSizeBytes": 20480,
  "TransferDurationMs": 34,
  "EncryptionTimeMs": 12
}
```

## Exemple d'entrée XML

```xml
<LogEntry>
  <Timestamp>2026-05-13T09:42:11Z</Timestamp>
  <JobName>backup-docs</JobName>
  <SourceFile>D:\Documents\rapport.txt</SourceFile>
  <TargetFile>E:\Backup\rapport.txt</TargetFile>
  <FileSizeBytes>20480</FileSizeBytes>
  <TransferDurationMs>34</TransferDurationMs>
  <EncryptionTimeMs>12</EncryptionTimeMs>
</LogEntry>
```
