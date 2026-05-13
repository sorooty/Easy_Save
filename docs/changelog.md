# Changelog

Historique de toutes les modifications du projet EasySave, de la mise en place du socle jusqu'aux derniers correctifs v2.0.

---

## v2.0 — Interface graphique WPF

### `2026-05-13` — Édition inline des travaux
**Branche :** `feature/edit-job`  
Ajout de l'édition inline pour chaque travail depuis sa carte dans `JobListView`. Le bouton ✏️ bascule vers un formulaire pré-rempli (nom, source, cible, type) sans changer de vue. Confirmation persistée dans `jobs.json` via l'événement `EditConfirmed`.

### `2026-05-13` — Correction barre de progression
**Branche :** `hotfix/progress-bar`  
Les stratégies (`FullSaveStrategy`, `DifferentialSaveStrategy`) n'appelaient pas `progress?.Report(state)` par fichier — seulement au début et à la fin. Ajout du paramètre `IProgress<SaveState>?` dans `ISaveStrategy.ExecuteSaveJob` et appel après chaque fichier copié.

### `2026-05-13` — Correctifs post-tests manuels
**Branche :** `feature/changements-mineurs-fix-v2.0`  
- Bouton **Ouvrir dossier logs** dans la vue Paramètres
- Détection du logiciel métier mid-run (pas seulement au lancement)

### `2026-05-13` — Intégration CryptoSoft
**Branche :** `feature/cryptosoft-integration`  
Implémentation de `CryptoService` : lance `CryptoSoft.exe` en sous-processus, mesure le temps, stocke `EncryptionTimeMs` dans les logs.

### `2026-05-13` — Logiciel métier (mid-run)
**Branche :** `feature/business-app-blocker`  
Implémentation de `BusinessAppWatcher` et intégration dans `SaveExecutor` pour bloquer le lancement et arrêter proprement une sauvegarde en cours.

### `2026-05-12` — Socle WPF + MVVM
**Branche :** `feature/wpf-scaffold`  
Création du projet `EasySave.WPF`, mise en place de l'architecture MVVM (`BaseViewModel`, `RelayCommand`, navigation), vues `JobListView` et `SettingsView`, localisation FR/EN via `.resx`.

### `2026-05-12` — Thème clair, Dropbox support
Corrections visuelles et support des lecteurs réseau/Dropbox comme destination.

### `2026-05-12` — Corrections d'incohérences
Fix de plusieurs incohérences détectées lors de la revue de code inter-équipe.

---

## v1.1 — XML Logger

### `2026-05-11` — XML Logger & LoggerFactory
**Branche :** `feature/v1.1-xml-logger`  
Ajout de `XmlLogger` et `LoggerFactory` dans `EasyLog.dll`. Option 6 Paramètres dans la console pour basculer JSON/XML. Sauvegarde du choix dans `settings.json`.

---

## v1.0 — Application Console

### `2026-05-06` — Fix PR #17 (revue de code)
Corrections suite à la revue de code : nommage, redondances, qualité.

### `2026-05-04` — Implémentation Core v1.0
Implémentation complète de `EasySave.Core` : `SaveExecutor`, stratégies `FullSaveStrategy` et `DifferentialSaveStrategy`, `ConfigService`, `StateService`, `AppPaths`.

### `2026-04-27` — Refactoring & conventions de nommage
Renommage de toutes les classes selon les conventions ProSoft (`Save*` prefix), suppression des doublons, mise à jour des diagrammes UML.

### `2026-04-27` — Audit design patterns
Revue des patterns utilisés : Strategy pour les stratégies de sauvegarde, Factory pour les loggers.

### `2026-04-23` — Implémentation v1.0 (blocs 2–5)
Implémentation de la vue console (`ConsoleView`), des modèles (`SaveJob`, `SaveState`, `SaveType`), et de `EasyLog.dll` v1.0 (`JsonLogger`).

### `2026-04-22` — Socle du projet
Création de la solution Visual Studio, des projets `EasyLog`, `EasySave.Core`, `EasySave.Console`, `EasySave.Tests`. Mise en place de la structure de dossiers et des conventions de travail.
