# Docker et Logs Centralisés

Gestion du serveur de logs centralisés via Docker.

---

## Démarrer le serveur

### Via l'interface WPF

1. Ouvrir l'onglet **Settings**
2. Section **Docker Container Control**
3. Cliquer le bouton **Start Docker**
4. Attendre le message "Docker started successfully"

### Via la ligne de commande

```powershell
cd C:\chemin\vers\Easy_Save
docker-compose up --build -d
```

---

## Vérifier le statut

Afficher l'état du conteneur:

```powershell
docker-compose ps
```

Résultat attendu:
```
NAME                    STATUS
easy_save-logserver     Up X minutes
```

---

## Consulter les logs Docker

Voir les logs en direct du serveur:

```powershell
docker-compose logs -f logserver
```

Arrêter le suivi: Ctrl+C

---

## Accéder au dashboard

Ouvrir http://localhost:5275/logs dans un navigateur.

Le dashboard affiche:
- Historique de tous les logs reçus
- Nombre de transferts par travail
- Filtrage par date et nom de travail

---

## Configurer l'application pour Docker

1. Onglet **Settings**
2. **Log Storage Mode**: sélectionner "Centralized (Docker)"
3. **Endpoint**: `http://localhost:5275` (par défaut)
4. Cliquer "Save"

Les logs seront envoyés au serveur Docker lors des sauvegardes.

---

## Modes de stockage des logs

Trois options disponibles:

| Mode | Fichiers locaux | Docker | Cas d'usage |
|------|---|---|---|
| Local Only | Oui | Non | Machine unique, tests locaux |
| Centralized (Docker) | Non | Oui | Environnement réseau, multi-utilisateurs |
| Both | Oui | Oui | Audit complet, redundance |

Sélectionner le mode approprié dans Settings.

---

## Arrêter le serveur

### Via l'interface WPF

1. Settings
2. Docker Container Control
3. Cliquer **Stop Docker**

### Via la ligne de commande

```powershell
docker-compose down
```

---

## Dépannage

### Docker refuse la connexion

Vérifier que Docker Desktop est lancé:
```powershell
docker ps
```

Si erreur: relancer Docker Desktop.

### Port 5275 déjà utilisé

Un autre service utilise le port. Options:
1. Arrêter le service conflictuel
2. Modifier le port dans `docker-compose.yml`:
   ```yaml
   ports:
     - "5276:5275"  # Port externe:interne
   ```
3. Mettre à jour l'endpoint dans Settings: `http://localhost:5276`

### Les logs n'arrivent pas

Vérifier:
1. Log Storage Mode = "Centralized (Docker)" dans Settings
2. Le conteneur tourne: `docker-compose ps`
3. L'endpoint est correct: `http://localhost:5275` (par défaut)
4. Pas de firewall bloquant le port 5275

Consulter les logs Docker pour les erreurs:
```powershell
docker-compose logs logserver | tail -50
```

### Réinitialiser le serveur

Arrêter et relancer:
```powershell
docker-compose down
docker-compose up --build -d
```

---

## Exemple complet

Démarrer l'app avec Docker:

```powershell
# Compiler le serveur
cd C:\chemin\vers\Easy_Save
dotnet build EasySave.LogServer/EasySave.LogServer.csproj -c Release

# Démarrer Docker
docker-compose up --build -d

# Lancer l'app WPF
dotnet run --project EasySave.WPF

# Dans l'interface: Settings > Log Storage Mode > "Centralized (Docker)"
# Puis exécuter un travail de sauvegarde

# Consulter les logs
docker-compose logs -f logserver
# Ou via le dashboard: http://localhost:5275/logs
```
