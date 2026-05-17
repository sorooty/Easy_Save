# Créer et gérer les travaux de sauvegarde

Guide complet pour configurer et exécuter les travaux de sauvegarde.

---

## Créer un nouveau travail

### Accès

1. Onglet **Jobs** dans la navigation principale
2. Bouton **Create New Job**

### Remplir le formulaire

| Champ | Description | Exemple |
|---|---|---|
| **Job Name** | Nom unique du travail | "Backup Documents" |
| **Source** | Dossier à sauvegarder | `C:\Users\John\Documents` |
| **Destination** | Où copier les fichiers | `D:\Backups\Documents` |
| **Type** | Full ou Differential | Voir tableau ci-dessous |

### Sélectionner le type de sauvegarde

| Type | Comportement | Quand l'utiliser |
|---|---|---|
| **Full** | Copie TOUS les fichiers, peu importe l'état cible | Sauvegarde initiale, sauvegarde complète |
| **Differential** | Copie uniquement les fichiers modifiés ou nouveaux | Sauvegarde régulière (plus rapide) |

### Exemple

**Sauvegarde initiale:**
1. Créer un travail type Full
2. Exécuter (copie tout)

**Sauvegarde suivante:**
1. Créer un travail type Differential
2. Exécuter (copie uniquement les changements)

### Valider et enregistrer

Cliquer le bouton **Save**. Le travail apparaît dans la liste.

---

## Lister et sélectionner les travaux

Une liste affiche tous les travaux créés:
- **Nom du travail**
- **Source** et **Destination**
- **Type** (Full ou Differential)
- **Date de création**

Cliquer sur un travail pour le sélectionner et voir ses détails.

---

## Exécuter un travail

### Démarrer la sauvegarde

1. Sélectionner le travail dans la liste
2. Bouton **Run**
3. Attendre la fin (barre de progression visible)

### Suivre la progression

Pendant l'exécution:
- Barre de progression: pourcentage complété
- Fichier en cours: nom du fichier actuellement transféré
- Temps écoulé et temps estimé
- Statut en temps réel

### Résultat final

À la fin:
- Message "Backup completed successfully" (succès)
- Ou message d'erreur avec détails
- Résumé: nombre de fichiers, taille totale, durée

---

## Contrôler l'exécution

### Play / Pause / Stop

Pendant l'exécution, trois boutons contrôlent le travail:

| Bouton | Action | Effet |
|---|---|---|
| **Play** | Démarrer ou reprendre | Reprend après pause |
| **Pause** | Mettre en pause | S'arrête après fichier en cours (pas de troncature) |
| **Stop** | Arrêter immédiatement | Arrêt brutal du travail |

### Exemple

Mettre en pause un travail volumineux:
1. Cliquer **Pause** pendant l'exécution
2. Le travail se termine gracieusement (fichier en cours complété)
3. Cliquer **Play** pour reprendre
4. Ou cliquer **Stop** pour annuler

---

## Consulter l'historique

Une fois exécuté, le travail apparaît dans l'onglet **Logs** avec:
- Timestamp
- Nom du travail
- Nombre de fichiers transférés
- Taille totale
- Durée
- Statut (OK ou erreur)

---

## Gestion des fichiers prioritaires

Si le travail contient des fichiers avec extensions prioritaires:

1. Ces fichiers sont traités EN PRIORITÉ
2. Les autres travaux ATTENDENT que tous les fichiers prioritaires soient complétés
3. Puis le reste des fichiers peut s'exécuter en parallèle

Configuration: Settings > Extensions prioritaires

Exemple:
- Extensions prioritaires: `.exe`, `.dll`
- Travail 1 contient `.exe` (prioritaire)
- Travail 2 contient `.txt` (normal)
- Travail 1 commence, transfert `.exe`
- Travail 2 attend
- `.exe` complété, puis Travail 2 commence

---

## Gestion des fichiers volumineux

Si le travail contient des fichiers volumineux (> au seuil configurable):

1. Maximum UN fichier volumineux peut être transféré à la fois
2. Les autres travaux peuvent continuer avec des fichiers petits
3. Seuil configurable dans Settings (par défaut: 10 MB)

Exemple:
- Seuil: 10 MB
- Travail 1: fichier 50 MB
- Travail 2: fichier 5 MB
- Travail 1 transfère le 50 MB (seul)
- Travail 2 transfère le 5 MB EN PARALLÈLE
- Impossible: Deux fichiers > 10 MB en même temps

---

## Éditer un travail

### Modifier les paramètres

1. Cliquer sur le travail dans la liste
2. Cliquer **Edit**
3. Modifier les champs (Source, Destination, Type)
4. Cliquer **Save**

Les modifications prennent effet au prochain **Run**.

---

## Supprimer un travail

1. Cliquer sur le travail dans la liste
2. Cliquer **Delete**
3. Confirmer la suppression

Le travail est supprimé de la liste. Les logs historiques restent accessibles.

---

## Dépannage

### "Access Denied" lors de l'exécution

Vérifier:
- Permissions de lecture sur la source
- Permissions d'écriture sur la destination
- Pas de fichiers verrouillés en lecture

### "Source path not found"

Vérifier:
- Le chemin source existe et est accessible
- Le chemin est correct (pas de caractères invalides)

### Travail lent ou gelé

Possibilités:
- Disque source/destination saturé
- Antivirus ralentissant les transferts
- Réseau lent (si destination est réseau)

Mettre en pause et arrêter pour relancer.

### Trop de fichiers à sauvegarder

Pas de limite au nombre de fichiers. Si très volumineux (> 1 GB):
- Envisager plusieurs travaux plus petits
- Augmenter le "Large File Limit" si beaucoup de petits fichiers
- Réduire les extensions prioritaires si trop nombreuses
