# Accéder aux logs

EasySave génère un fichier log journalier pour chaque sauvegarde. Ce fichier contient une entrée par fichier transféré.

---

## Emplacement

| Fichier | Chemin |
|---|---|
| Log du jour (JSON) | `%AppData%\EasySave\logs\YYYY-MM-DD.json` |
| Log du jour (XML) | `%AppData%\EasySave\logs\YYYY-MM-DD.xml` |

**Depuis l'interface :** Paramètres → **📁 Ouvrir dossier logs**

---

## Format JSON

```json
[
  {
    "Timestamp": "2026-05-13T09:42:11Z",
    "JobName": "backup-docs",
    "SourceFile": "D:\\Documents\\rapport.txt",
    "TargetFile": "E:\\Backup\\rapport.txt",
    "FileSizeBytes": 20480,
    "TransferDurationMs": 34,
    "EncryptionTimeMs": 12
  },
  {
    "Timestamp": "2026-05-13T09:42:11Z",
    "JobName": "backup-docs",
    "SourceFile": "D:\\Documents\\config.json",
    "TargetFile": "E:\\Backup\\config.json",
    "FileSizeBytes": 512,
    "TransferDurationMs": 2,
    "EncryptionTimeMs": 0
  }
]
```

## Format XML

```xml
<Logs>
  <LogEntry>
    <Timestamp>2026-05-13T09:42:11Z</Timestamp>
    <JobName>backup-docs</JobName>
    <SourceFile>D:\Documents\rapport.txt</SourceFile>
    <TargetFile>E:\Backup\rapport.txt</TargetFile>
    <FileSizeBytes>20480</FileSizeBytes>
    <TransferDurationMs>34</TransferDurationMs>
    <EncryptionTimeMs>12</EncryptionTimeMs>
  </LogEntry>
</Logs>
```

---

## Interprétation des champs

| Champ | Type | Description |
|---|---|---|
| `Timestamp` | ISO 8601 | Horodatage du transfert |
| `JobName` | string | Nom du travail de sauvegarde |
| `SourceFile` | string | Chemin complet du fichier source (UNC) |
| `TargetFile` | string | Chemin complet du fichier destination (UNC) |
| `FileSizeBytes` | long | Taille du fichier en octets |
| `TransferDurationMs` | long | Durée du transfert en ms (**négatif = erreur**) |
| `EncryptionTimeMs` | long | `0` = pas crypté · `>0` = durée cryptage · `<0` = erreur CryptoSoft |

---

## État temps réel

`state.json` est mis à jour en continu pendant une sauvegarde. Il contient l'état de chaque travail.

```
%AppData%\EasySave\state.json
```

→ [Détail du fichier d'état](../technical/state-files.md)
