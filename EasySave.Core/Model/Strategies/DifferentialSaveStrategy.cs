using System.Diagnostics;
using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;

namespace EasySave.Core.Model.Strategies
{
    /// <summary>
    /// Sauvegarde différentielle : copie uniquement les fichiers absents de la cible
    /// ou dont la date de modification source est plus récente que la cible.
    /// </summary>
    public class DifferentialSaveStrategy : ISaveStrategy
    {
        private readonly ILogger _logger;
        private readonly IStateService _stateService;
        private readonly CryptoService _cryptoService;
        private readonly SettingsService _settingsService;

        public DifferentialSaveStrategy(ILogger logger, IStateService stateService,
            CryptoService cryptoService, SettingsService settingsService)
        {
            _logger = logger;
            _stateService = stateService;
            _cryptoService = cryptoService;
            _settingsService = settingsService;
        }

        public void ExecuteSaveJob(SaveJob job, CancellationToken cancellationToken = default)
        {
            var settings = _settingsService.LoadSettings();
            var allFiles = Directory.GetFiles(job.SourceFolder, "*", SearchOption.AllDirectories);

            // Pré-filtre : seuls les fichiers éligibles sont comptabilisés et copiés
            var filesToCopy = allFiles.Where(src => NeedsCopy(src, job)).ToArray();
            int totalFiles = filesToCopy.Length;
            long totalBytes = filesToCopy.Sum(f => new FileInfo(f).Length);
            int remaining = totalFiles;
            long remainingBytes = totalBytes;

            foreach (var sourceFile in filesToCopy)
            {
                string relativePath = Path.GetRelativePath(job.SourceFolder, sourceFile);
                string targetFile = Path.Combine(job.TargetFolder, relativePath);
                long fileSize = new FileInfo(sourceFile).Length;

                Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);

                _stateService.UpdateState(new SaveState
                {
                    Name = job.Name,
                    Status = "Active",
                    LastActionTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    TotalFiles = totalFiles,
                    TotalSizeBytes = totalBytes,
                    RemainingFiles = remaining,
                    RemainingFilesBytes = remainingBytes,
                    ProgressPercent = totalFiles == 0 ? 100 : (int)((totalFiles - remaining) * 100.0 / totalFiles),
                    CurrentSourceFile = sourceFile,
                    CurrentTargetFile = targetFile
                });

                long transferMs = -1;
                long encryptionTimeMs = 0;
                string error = string.Empty;

                try
                {
                    var sw = Stopwatch.StartNew();
                    File.Copy(sourceFile, targetFile, overwrite: true);
                    sw.Stop();
                    transferMs = sw.ElapsedMilliseconds;

                    if (_cryptoService.NeedsEncryption(targetFile, settings.EncryptedExtensions))
                        encryptionTimeMs = _cryptoService.Encrypt(targetFile, settings.CryptoSoftPath);
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }

                _logger.Log(new LogEntry(job.Name, sourceFile, targetFile, fileSize, transferMs,
                    state: error == string.Empty ? "OK" : "ERROR",
                    errorMessage: error,
                    encryptionTimeMs: encryptionTimeMs));

                remaining--;
                remainingBytes -= fileSize;

                // Finish the current file first, then honour a cancellation request (business software / user stop)
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.Log(new LogEntry(job.Name, string.Empty, string.Empty, 0, -1,
                        state: "STOPPED", errorMessage: "Job interrupted: business software detected"));
                    return;
                }
            }
        }

        /// <summary>
        /// Retourne true si le fichier source doit être copié :
        /// absent à la cible OU plus récent que la version existante.
        /// </summary>
        private static bool NeedsCopy(string sourceFile, SaveJob job)
        {
            string relativePath = Path.GetRelativePath(job.SourceFolder, sourceFile);
            string targetFile = Path.Combine(job.TargetFolder, relativePath);

            if (!File.Exists(targetFile))
                return true;

            return File.GetLastWriteTimeUtc(sourceFile) > File.GetLastWriteTimeUtc(targetFile);
        }
    }
}
