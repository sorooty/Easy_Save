namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Gère la langue de l'interface et fournit les chaînes traduites.
    /// Les traductions sont définies en dur ici pour éviter tout I/O supplémentaire.
    /// Pour ajouter une langue : ajouter une entrée dans _translations.
    /// </summary>
    public class LanguageService
    {
        private string _currentLanguage;

        // Structure : langue → (clé → texte traduit)
        private readonly Dictionary<string, Dictionary<string, string>> _translations;

        public LanguageService()
        {
            _currentLanguage = "en";
            _translations = new Dictionary<string, Dictionary<string, string>>
            {
                ["en"] = new()
                {
                    ["menu.title"]              = "=== EasySave ===",
                    ["menu.list"]               = "1. List backup jobs",
                    ["menu.add"]                = "2. Add a backup job",
                    ["menu.remove"]             = "3. Remove a backup job",
                    ["menu.run"]                = "4. Run backup job(s)",
                    ["menu.language"]           = "5. Change language/Changer de langue",
                    ["menu.settings"]           = "6. Settings",
                    ["menu.quit"]               = "7. Quit",
                    ["menu.choice"]             = "Your choice: ",
                    ["job.list"]                = "=== List of backup jobs ===",
                    ["job.name"]                = "Job name: ",
                    ["job.source"]              = "Source folder: ",
                    ["job.target"]              = "Target folder: ",
                    ["job.type"]                = "Type (1=Full, 2=Differential): ",
                    ["job.added"]               = "Job added successfully.",
                    ["job.removed"]             = "Job removed successfully.",
                    ["job.not_found"]           = "Job not found.",
                    ["job.found"]               = "Available work :",
                    ["job.no_jobs"]             = "No backup jobs defined.",
                    ["job.run_which"]           = "Enter an execution command (type ':help' to see the syntax)  ",
                    ["job.max_reached"]         = "Maximum number of jobs reached (5).",
                    ["run.starting"]            = "Starting: ",
                    ["run.completed"]           = "Completed: ",
                    ["run.failed"]              = "Failed: ",
                    ["error.invalid_choice"]    = "Invalid choice. Please try again.",
                    ["error.source_not_found"]  = "Source folder does not exist.",
                    ["error.target_not_found"]  = "Target folder does not exist.",
                    ["language.prompt"]         = "Select language (en/fr): ",
                    ["language.changed"]        = "Language changed.",
                    ["close.application"]       = "Close the app...",
                    ["command.cancel"]          = "Type ':cancel' to return to the main menu.",
                    ["execute.help"]            = "Syntax:\n1     : execute job 1\n1-3   : execute jobs 1 to 3\n1;3   : execute jobs 1 and 3",
                    ["job.status"]              = "Status",
                    ["status.ready"]            = "Ready",
                    ["status.running"]          = "Running...",
                    ["status.done"]             = "Done",
                    ["status.error"]            = "Error",
                    ["job.invalid"]             = "Invalid job - check required fields.",
                    ["job.success"]             = "Backup completed successfully.",
                    ["job.error"]               = "Backup failed: ",
                    ["settings.title"]          = "=== Settings ===",
                    ["settings.log_format.current"]  = "Current log format: ",
                    ["settings.log_format.prompt"]   = "Log format (1=JSON, 2=XML): ",
                    ["settings.log_format.changed"]  = "Log format saved. Will take effect on next launch.",
                    ["form.error.name_required"]     = "Name is required.",
                    ["form.error.source_required"]   = "Source folder is required.",
                    ["form.error.target_required"]   = "Target folder is required.",
                    ["form.error.name_exists"]       = "A job with this name already exists.",
                    ["job.blocked_by_business_app"]  = "Business software is running. Backup is blocked.",
                    ["status.stopped"]              = "Stopped",
                    ["job.stopped"]                 = "Backup stopped.",
                    ["settings.saved"]               = "Settings saved.",
                    ["settings.restarting"]          = "Language changed — restarting...",
                },
                ["fr"] = new()
                {
                    ["menu.title"]              = "=== EasySave ===",
                    ["menu.list"]               = "1. Lister les travaux de sauvegarde",
                    ["menu.add"]                = "2. Ajouter un travail de sauvegarde",
                    ["menu.remove"]             = "3. Supprimer un travail de sauvegarde",
                    ["menu.run"]                = "4. Exécuter un/des travail(s)",
                    ["menu.language"]           = "5. Change language/Changer de langue",
                    ["menu.settings"]           = "6. Paramètres",
                    ["menu.quit"]               = "7. Quitter",
                    ["menu.choice"]             = "Votre choix : ",
                    ["job.list"]                = "=== Liste des travaux de sauvegarde ===",
                    ["job.name"]                = "Nom du travail : ",
                    ["job.source"]              = "Dossier source : ",
                    ["job.target"]              = "Dossier cible : ",
                    ["job.type"]                = "Type (1=Complète, 2=Différentielle) : ",
                    ["job.added"]               = "Travail ajouté avec succès.",
                    ["job.removed"]             = "Travail supprimé avec succès.",
                    ["job.not_found"]           = "Travail introuvable.",
                    ["job.found"]               = "Travaux disponible :",
                    ["job.no_jobs"]             = "Aucun travail de sauvegarde défini.",
                    ["job.run_which"]           = "Entrer une commande d'execution (taper ':help' pour afficher la syntaxe) ",
                    ["job.max_reached"]         = "Nombre maximum de travaux atteint (5).",
                    ["run.starting"]            = "Démarrage : ",
                    ["run.completed"]           = "Terminé : ",
                    ["run.failed"]              = "Échec : ",
                    ["error.invalid_choice"]    = "Choix invalide. Veuillez réessayer.",
                    ["error.source_not_found"]  = "Le dossier source n'existe pas.",
                    ["error.target_not_found"]  = "Le dossier cible n'existe pas.",
                    ["language.prompt"]         = "Choisir la langue (en/fr) : ",
                    ["language.changed"]        = "Langue modifiée.",
                    ["close.application"]       = "Fermeture de l'application...",
                    ["command.cancel"]          = "Tapez ':cancel' pour revenir au menu principal.",
                    ["execute.help"]            = "\nSyntaxe :\n1     : exécuter le travail 1\n1-3   : exécuter les travaux 1 à 3\n1;3   : exécuter les travaux 1 et 3",
                    ["job.status"]              = "Statut",
                    ["status.ready"]            = "Prêt",
                    ["status.running"]          = "En cours...",
                    ["status.done"]             = "Terminé",
                    ["status.error"]            = "Erreur",
                    ["job.invalid"]             = "Job invalide - Vérifiez les champs obligatoires.",
                    ["job.success"]             = "Sauvegarde réussie avec succès.",
                    ["job.error"]               = "Erreur lors de la sauvegarde : ",
                    ["settings.title"]          = "=== Paramètres ===",
                    ["settings.log_format.current"]  = "Format de log actuel : ",
                    ["settings.log_format.prompt"]   = "Format de log (1=JSON, 2=XML) : ",
                    ["settings.log_format.changed"]  = "Format de log enregistré. Prendra effet au prochain lancement.",
                    ["form.error.name_required"]     = "Le nom est obligatoire.",
                    ["form.error.source_required"]   = "Le dossier source est obligatoire.",
                    ["form.error.target_required"]   = "Le dossier cible est obligatoire.",
                    ["form.error.name_exists"]       = "Un job avec ce nom existe déjà.",
                    ["job.blocked_by_business_app"]  = "Le logiciel métier est en cours. La sauvegarde est bloquée.",
                    ["status.stopped"]              = "Arrêté",
                    ["job.stopped"]                 = "Sauvegarde arrêtée.",
                    ["settings.saved"]               = "Paramètres enregistrés.",
                    ["settings.restarting"]          = "Langue modifiée — redémarrage...",
                }
            };
        }

        public void SetLanguage(string languageCode)
        {
            // Accepte uniquement les codes de langue connus
            if (_translations.ContainsKey(languageCode))
                _currentLanguage = languageCode;
        }

        /// <summary>
        /// Retourne le texte traduit pour la clé donnée.
        /// Si la clé est introuvable, retourne la clé entre crochets pour faciliter le débogage.
        /// </summary>
        public string GetText(string key)
        {
            if (_translations.TryGetValue(_currentLanguage, out var lang) &&
                lang.TryGetValue(key, out var text))
                return text;

            return $"[{key}]";
        }

        public string GetCurrentLanguage() => _currentLanguage;
    }
}
