# Logiciel métier — Détection et blocage

---

## Principe

Un **logiciel métier** (ERP, comptabilité, etc.) utilise intensivement le réseau ou les disques. Lancer une sauvegarde simultanément peut provoquer des conflits. EasySave détecte sa présence et adapte son comportement.

---

## Configuration

Dans **Paramètres généraux → Logiciel métier** :

```
calc.exe           (calculatrice — pour les démonstrations)
erp.exe            (exemple réel)
```

Seul le nom de l'exécutable est nécessaire, pas le chemin complet.

---

## `BusinessAppWatcher`

```csharp
// EasySave.Core/Services/BusinessAppWatcher.cs
public class BusinessAppWatcher
{
    private readonly string _processName;

    // Retourne true si le processus est actif en ce moment
    public bool IsRunning()
        => Process.GetProcessesByName(_processName).Length > 0;
}
```

---

## Comportements en v2.0

| Situation | Comportement |
|---|---|
| Lancement d'un travail — logiciel métier actif | **Blocage immédiat** — le travail ne démarre pas |
| Logiciel métier lancé **en cours** de sauvegarde | **Termine le fichier en cours**, puis s'arrête proprement |
| Arrêt forcé | Consigné dans le fichier log (`TransferDurationMs < 0`) |

### Implémentation dans `SaveExecutor`

```csharp
// Avant chaque fichier :
if (_businessWatcher?.IsRunning() == true)
{
    state.IsActive = false;
    stateService.UpdateState(state);
    return false;  // arrêt propre
}
```

---

## Évolution en v3.0

Le comportement change en v3.0 :

| Version | Comportement |
|---|---|
| v2.0 | Blocage au lancement · arrêt après fichier en cours |
| v3.0 | **Pause automatique** de tous les travaux · **reprise automatique** dès la fermeture |

`BusinessAppWatcher` passera en mode polling continu et émettra un événement vers les threads de sauvegarde.

---

## Test avec la calculatrice Windows

La calculatrice (`calc.exe`) est recommandée pour les démonstrations :

1. Paramètres → Logiciel métier : `calc.exe`
2. Lancer une sauvegarde sur un gros dossier
3. Ouvrir la Calculatrice Windows pendant l'exécution
4. ✅ La sauvegarde doit s'arrêter après le fichier en cours
