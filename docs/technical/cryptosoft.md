# CryptoSoft — Intégration du cryptage

---

## Présentation

**CryptoSoft** est un exécutable externe développé séparément. EasySave ne réimplémente pas le cryptage — il lance CryptoSoft en sous-processus pour chaque fichier éligible.

---

## Algorithme

CryptoSoft utilise un **XOR par clé cyclique** :

```
octet_chiffré = octet_source XOR octet_clé[i % longueur_clé]
```

- La clé est répétée cycliquement si le fichier est plus long
- XOR est **symétrique** : appliquer deux fois avec la même clé redonne le fichier original
- CryptoSoft sert donc à la fois au chiffrement et au déchiffrement

---

## Intégration dans EasySave

### Configuration (Paramètres généraux)

1. Renseigner le **chemin vers `CryptoSoft.exe`**
2. Définir la **liste des extensions à crypter** (ex. `.txt .docx .pdf`)

### Exécution

Pour chaque fichier copié dont l'extension est dans la liste :

```
CryptoSoft.exe <chemin_source> <chemin_destination>
```

`CryptoService` mesure le temps d'exécution du processus en ms.

---

## `CryptoService`

```csharp
// EasySave.Core/Services/CryptoService.cs
public class CryptoService
{
    private readonly string _cryptoSoftPath;
    private readonly IEnumerable<string> _extensions;

    // Lance CryptoSoft.exe, retourne le temps en ms ou un code d'erreur négatif
    public long Encrypt(string sourcePath, string destinationPath);

    // Retourne true si l'extension du fichier est dans la liste configurée
    public bool ShouldEncrypt(string filePath);
}
```

---

## `EncryptionTimeMs` dans les logs

| Valeur | Signification |
|---|---|
| `0` | Fichier non crypté (extension non concernée) |
| `> 0` | Temps de cryptage réel en millisecondes |
| `< 0` | Code d'erreur retourné par CryptoSoft |

---

## Flux dans une stratégie

```csharp
// Dans FullSaveStrategy / DifferentialSaveStrategy
File.Copy(source, target, overwrite: true);

long encryptionMs = 0;
if (cryptoService?.ShouldEncrypt(source) == true)
    encryptionMs = cryptoService.Encrypt(source, target);

logger.Log(new LogEntry { ..., EncryptionTimeMs = encryptionMs });
progress?.Report(state);
```

---

## Problèmes courants

| Problème | Cause probable | Solution |
|---|---|---|
| `EncryptionTimeMs < 0` dans les logs | CryptoSoft a retourné une erreur | Vérifier le chemin et les droits sur les fichiers |
| Cryptage jamais déclenché | Extension non ajoutée dans les paramètres | Ajouter l'extension dans Paramètres → Extensions à crypter |
| `CryptoSoft.exe introuvable` | Chemin incorrect | Vérifier Paramètres → Chemin CryptoSoft |
