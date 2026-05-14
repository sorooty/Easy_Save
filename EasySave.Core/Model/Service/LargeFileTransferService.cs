using EasySave.Core.Model.Entities;

namespace EasySave.Core.Model.Service
{
    public class LargeFileTransferService
    {
        private readonly GeneralSettings _settings;
        private readonly SemaphoreSlim _largeFileSemaphore;

        public LargeFileTransferService(GeneralSettings settings)
        {
            _settings = settings;
            _largeFileSemaphore = new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// Vérifie si le fichier dépasse la limite définie dans les paramètres généraux.
        /// </summary>
        public bool IsLargeFile(string filePath)
        {
            long limitBytes = _settings.LargeFileLimitKo * 1024;
            long fileSize = new FileInfo(filePath).Length;

            return limitBytes > 0 && fileSize >= limitBytes;
        }

        /// <summary>
        /// Exécute le transfert du fichier.
        /// Si le fichier est gros, un seul gros transfert est autorisé à la fois.
        /// </summary>
        public async Task ExecuteTransferAsync(
            string sourceFile,
            Func<Task> transferAction,
            CancellationToken cancellationToken = default)
        {
            if (!IsLargeFile(sourceFile))
            {
                await transferAction();
                return;
            }

            await _largeFileSemaphore.WaitAsync(cancellationToken);

            try
            {
                await transferAction();
            }
            finally
            {
                _largeFileSemaphore.Release();
            }
        }
    }
}