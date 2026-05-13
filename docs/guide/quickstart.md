# Premiers pas

Ce guide montre comment créer et exécuter votre première sauvegarde dans EasySave v2.0.

---

## Créer un travail de sauvegarde

1. Lancer EasySave (WPF)
2. Cliquer sur **+ Ajouter un travail**
3. Remplir le formulaire :

| Champ | Description | Exemple |
|---|---|---|
| Nom | Identifiant unique | `backup-documents` |
| Dossier source | Chemin absolu du dossier à sauvegarder | `D:\Documents` |
| Dossier destination | Chemin absolu où copier | `E:\Sauvegardes\documents` |
| Type | Complète ou Différentielle | Complète |

4. Cliquer sur **Enregistrer**

Le travail apparaît dans la liste.

---

## Types de sauvegarde

| Type | Comportement | Quand l'utiliser |
|---|---|---|
| **Complète** | Copie tous les fichiers sans exception | Premier lancement, sauvegarde de référence |
| **Différentielle** | Copie uniquement les fichiers absents ou modifiés depuis la dernière copie | Sauvegardes régulières — plus rapide |

**Critère différentielle :** date de modification source > destination **ou** taille différente.

---

## Exécuter une sauvegarde

1. Cliquer sur la carte du travail pour l'ouvrir
2. Cliquer sur **▶ Exécuter**
3. La barre de progression se met à jour à chaque fichier copié
4. Le statut passe à `Terminé` en fin d'exécution

!!! warning "Logiciel métier"
    Si un logiciel métier est configuré et en cours d'exécution, le travail ne démarrera pas. Fermez le logiciel métier et réessayez.

---

## Modifier un travail

1. Ouvrir la carte du travail
2. Cliquer sur le bouton **✏️ Modifier**
3. Modifier les champs souhaités
4. Cliquer sur **💾 Enregistrer** pour confirmer, ou **✕ Annuler** pour revenir

---

## Supprimer un travail

1. Ouvrir la carte du travail
2. Cliquer sur **🗑 Supprimer**
3. Confirmer la suppression

---

## Vérifier les résultats

Après une sauvegarde, vous pouvez :

- Consulter les fichiers copiés dans le dossier destination
- Ouvrir le dossier de logs depuis **Paramètres → Ouvrir dossier logs**
- Voir le log du jour : `%AppData%\EasySave\logs\YYYY-MM-DD.json`

→ [Accéder aux logs](logs.md)
