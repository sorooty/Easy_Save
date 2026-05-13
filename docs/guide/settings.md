# Paramètres généraux

Accessible depuis l'onglet **Paramètres** dans la barre de navigation principale.

---

## Format de log

Choisir entre **JSON** (défaut) et **XML**.

| Format | Extension fichier | Avantage |
|---|---|---|
| JSON | `.json` | Lisible dans n'importe quel éditeur, parseable facilement |
| XML | `.xml` | Compatible avec des outils d'import XML / Excel |

Le changement prend effet immédiatement pour les prochaines sauvegardes.

---

## Extensions de fichiers à crypter

Liste des extensions que CryptoSoft doit traiter lors des sauvegardes.

**Exemples :** `.txt`, `.docx`, `.pdf`, `.xlsx`

- Saisir chaque extension **avec le point** (ex. `.txt`)
- Séparer par des virgules ou des espaces
- Laisser vide pour désactiver le cryptage

!!! info
    Seuls les fichiers dont l'extension est dans cette liste seront cryptés. Les autres sont copiés tels quels.

---

## Chemin vers CryptoSoft.exe

Chemin absolu vers l'exécutable CryptoSoft.

```
C:\Outils\CryptoSoft\CryptoSoft.exe
```

- Cliquer sur **Parcourir** pour sélectionner le fichier via l'explorateur
- Laisser vide pour désactiver le cryptage

→ [Fonctionnement de CryptoSoft](../technical/cryptosoft.md)

---

## Logiciel métier

Nom du processus à surveiller (sans chemin, juste le nom de l'exécutable).

**Exemples :**
- `calc.exe` — calculatrice Windows (pour les démonstrations)
- `erp.exe` — logiciel ERP d'entreprise
- `compta.exe`

Quand ce processus est détecté :
- Un nouveau travail **ne peut pas démarrer**
- Un travail en cours **termine le fichier actuel** puis s'arrête

Laisser vide pour désactiver la surveillance.

→ [Comportement détaillé](../technical/business-app.md)

---

## Ouvrir le dossier des logs

Bouton **📁 Ouvrir dossier logs** — ouvre `%AppData%\EasySave\logs\` dans l'explorateur Windows.

→ [Accéder aux logs](logs.md)
