# Installation

**Prérequis système :** Windows 10 / 11 · .NET 8.0 Runtime

---

## 1. Prérequis

### .NET 8.0 Runtime

Télécharger sur [dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0) → section **Run apps · Windows x64**.

Vérifier l'installation :

```powershell
dotnet --list-runtimes
# Doit afficher Microsoft.NETCore.App 8.x.x
```

### CryptoSoft *(optionnel, nécessaire pour le cryptage)*

EasySave ne livre pas CryptoSoft. Vous devez disposer d'un `CryptoSoft.exe` séparément et renseigner son chemin dans les [Paramètres généraux](settings.md).

---

## 2. Cloner le dépôt

```powershell
git clone https://github.com/sorooty/Easy_Save.git
cd Easy_Save
```

---

## 3. Builder la solution

```powershell
dotnet build
```

Un build propre doit afficher `Build succeeded. 0 Error(s)`.

---

## 4. Lancer EasySave

### Interface graphique (v2.0) — recommandé

```powershell
cd EasySave.WPF
dotnet run
```

Ou ouvrir `Easy_Save.sln` dans **Visual Studio 2022** et appuyer sur F5.

### Interface console (v1.0 / v1.1)

```powershell
cd EasySave.Console
dotnet run
```

---

## 5. Fichiers générés au premier lancement

Au premier démarrage, EasySave crée automatiquement les dossiers nécessaires :

| Fichier | Chemin |
|---|---|
| Configuration travaux | `%AppData%\EasySave\jobs.json` |
| Paramètres | `%AppData%\EasySave\settings.json` |
| État temps réel | `%AppData%\EasySave\state.json` |
| Dossier de logs | `%AppData%\EasySave\logs\` |

> `%AppData%` = `C:\Users\<nom>\AppData\Roaming`

---

## 6. Lancer les tests

```powershell
dotnet test
```

---

## Problèmes courants

| Problème | Solution |
|---|---|
| `dotnet: command not found` | Installer .NET 8.0 SDK |
| `MSB3027` — DLL locked | Fermer l'application WPF avant de rebuilder |
| Cryptage ne fonctionne pas | Vérifier le chemin `CryptoSoft.exe` dans les Paramètres |
| Fenêtre blanche au démarrage | Vérifier que les fichiers `.resx` sont bien inclus dans le build |
