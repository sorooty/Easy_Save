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
                    ["menu.language"]           = "5. Change language",
                    ["menu.quit"]               = "6. Quit",
                    ["menu.choice"]             = "Your choice: ",
                    ["job.name"]                = "Job name: ",
                    ["job.source"]              = "Source folder: ",
                    ["job.target"]              = "Target folder: ",
                    ["job.type"]                = "Type (1=Full, 2=Differential): ",
                    ["job.added"]               = "Job added successfully.",
                    ["job.removed"]             = "Job removed successfully.",
                    ["job.not_found"]           = "Job not found.",
                    ["job.no_jobs"]             = "No backup jobs defined.",
                    ["job.run_which"]           = "Run which job? (number, or 'all'): ",
                    ["job.max_reached"]         = "Maximum number of jobs reached (5).",
                    ["run.starting"]            = "Starting: ",
                    ["run.completed"]           = "Completed: ",
                    ["run.failed"]              = "Failed: ",
                    ["error.invalid_choice"]    = "Invalid choice. Please try again.",
                    ["error.source_not_found"]  = "Source folder does not exist.",
                    ["error.target_not_found"]  = "Target folder does not exist.",
                    ["language.prompt"]         = "Select language (en/fr): ",
                    ["language.changed"]        = "Language changed.",
                },
                ["fr"] = new()
                {
                    ["menu.title"]              = "=== EasySave ===",
                    ["menu.list"]               = "1. Lister les travaux de sauvegarde",
                    ["menu.add"]                = "2. Ajouter un travail de sauvegarde",
                    ["menu.remove"]             = "3. Supprimer un travail de sauvegarde",
                    ["menu.run"]                = "4. Exécuter un/des travail(s)",
                    ["menu.language"]           = "5. Changer de langue",
                    ["menu.quit"]               = "6. Quitter",
                    ["menu.choice"]             = "Votre choix : ",
                    ["job.name"]                = "Nom du travail : ",
                    ["job.source"]              = "Dossier source : ",
                    ["job.target"]              = "Dossier cible : ",
                    ["job.type"]                = "Type (1=Complète, 2=Différentielle) : ",
                    ["job.added"]               = "Travail ajouté avec succès.",
                    ["job.removed"]             = "Travail supprimé avec succès.",
                    ["job.not_found"]           = "Travail introuvable.",
                    ["job.no_jobs"]             = "Aucun travail de sauvegarde défini.",
                    ["job.run_which"]           = "Quel travail exécuter ? (numéro, ou 'all') : ",
                    ["job.max_reached"]         = "Nombre maximum de travaux atteint (5).",
                    ["run.starting"]            = "Démarrage : ",
                    ["run.completed"]           = "Terminé : ",
                    ["run.failed"]              = "Échec : ",
                    ["error.invalid_choice"]    = "Choix invalide. Veuillez réessayer.",
                    ["error.source_not_found"]  = "Le dossier source n'existe pas.",
                    ["error.target_not_found"]  = "Le dossier cible n'existe pas.",
                    ["language.prompt"]         = "Choisir la langue (en/fr) : ",
                    ["language.changed"]        = "Langue modifiée.",
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
