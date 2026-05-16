using EasySave.Core.Model.Entities;
using System.Collections.Concurrent;

namespace EasySave.Core.Model.Service
{
    public class PriorityFileService
    {
        private readonly GeneralSettings _settings;
        private readonly ConcurrentDictionary<string, byte> _pendingFiles;

        public PriorityFileService(GeneralSettings settings)
        {
            _settings = settings;
            _pendingFiles = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Vérifie si un fichier est prioritaire.
        /// </summary>
        public bool IsPriorityFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            return _settings.PriorityExtensions
                .Any(ext => string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Vérifie s'il reste des fichiers prioritaires en attente.
        /// </summary>
        public bool HasPendingPriorityFiles()
        {
            return _pendingFiles.Keys.Any(IsPriorityFile);
        }

        /// <summary>
        /// Ajoute un fichier à la liste d'attente (thread-safe).
        /// </summary>
        public void AddPendingFile(string filePath)
        {
            _pendingFiles.TryAdd(filePath, 0);
        }

        /// <summary>
        /// Retire un fichier traité de la liste d'attente (thread-safe).
        /// </summary>
        public void RemovePendingFile(string filePath)
        {
            _pendingFiles.TryRemove(filePath, out _);
        }
    }
}
