using EasySave.Core.Model.Entities;

namespace EasySave.Core.Model.Service
{
    public class PriorityFileService
    {
        private readonly GeneralSettings _settings;
        private readonly List<string> _pendingFiles;

        public PriorityFileService(GeneralSettings settings)
        {
            _settings = settings;
            _pendingFiles = new List<string>();
        }

        /// <summary>
        /// Vérifie si un fichier est prioritaire.
        /// </summary>
        public bool IsPriorityFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            return _settings.PriorityExtensions
                .Select(ext => ext.ToLower())
                .Contains(extension);
        }

        /// <summary>
        /// Vérifie s'il reste des fichiers prioritaires en attente.
        /// </summary>
        public bool HasPendingPriorityFiles()
        {
            return _pendingFiles.Any(file => IsPriorityFile(file));
        }

        /// <summary>
        /// Ajoute un fichier à la liste d'attente.
        /// </summary>
        public void AddPendingFile(string filePath)
        {
            if (!_pendingFiles.Contains(filePath))
            {
                _pendingFiles.Add(filePath);
            }
        }

        /// <summary>
        /// Retire un fichier traité de la liste d'attente.
        /// </summary>
        public void RemovePendingFile(string filePath)
        {
            _pendingFiles.Remove(filePath);
        }
    }
}
