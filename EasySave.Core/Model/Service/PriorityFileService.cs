using EasySave.Core.Model.Entities;
using System.Collections.Concurrent;

namespace EasySave.Core.Model.Service
{
    public class PriorityFileService
    {
        private readonly GeneralSettings _settings;
        private readonly ConcurrentDictionary<string, byte> _pendingFiles;

        // Gate: open (true) when no priority files are pending, closed (false) otherwise.
        // Non-priority transfers must wait at this gate while priority files are being processed.
        private readonly ManualResetEventSlim _gate = new ManualResetEventSlim(initialState: true);

        public PriorityFileService(GeneralSettings settings)
        {
            _settings = settings;
            _pendingFiles = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Vérifie si un fichier est prioritaire selon les extensions configurées.
        /// </summary>
        public bool IsPriorityFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            return _settings.PriorityExtensions
                .Any(ext => string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Vérifie s'il reste des fichiers prioritaires en attente sur l'ensemble des jobs.
        /// </summary>
        public bool HasPendingPriorityFiles()
        {
            return _pendingFiles.Keys.Any(IsPriorityFile);
        }

        /// <summary>
        /// Ajoute un fichier à la liste d'attente. Ferme le gate si le fichier est prioritaire.
        /// </summary>
        public void AddPendingFile(string filePath)
        {
            _pendingFiles.TryAdd(filePath, 0);
            if (IsPriorityFile(filePath))
                _gate.Reset(); // close: non-priority transfers must wait
        }

        /// <summary>
        /// Retire un fichier traité. Rouvre le gate si plus aucun fichier prioritaire n'est en attente.
        /// </summary>
        public void RemovePendingFile(string filePath)
        {
            _pendingFiles.TryRemove(filePath, out _);
            if (!HasPendingPriorityFiles())
                _gate.Set(); // open: no more priority files, non-priority transfers may proceed
        }

        /// <summary>
        /// Bloque le thread appelant tant que des fichiers prioritaires sont en attente.
        /// Doit être appelé avant chaque transfert de fichier non-prioritaire.
        /// </summary>
        public void WaitForNonPriority(CancellationToken cancellationToken)
        {
            _gate.Wait(cancellationToken);
        }
    }
}
