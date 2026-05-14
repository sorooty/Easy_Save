using System.Diagnostics;
using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;

namespace EasySave.Core.Model.Strategies
{
    /// <summary>
    /// Sauvegarde complète : copie l'intégralité des fichiers du dossier source vers la cible.
    /// Chaque fichier copié est loggué et l'état global est mis à jour en temps réel.
    /// </summary>
    public class FullSaveStrategy : ISaveStrategy
    {
        private readonly ILogger _logger;
        private readonly IStateService _stateService;
        private readonly CryptoService _cryptoService;
        private readonly SettingsService _settingsService;

        public FullSaveStrategy(ILogger logger, IStateService stateService,
            CryptoService cryptoService, SettingsService settingsService)
        {
            _logger = logger;
            _stateService = stateService;
            _cryptoService = cryptoService;
            _settingsService = settingsService;
        }

        public void ExecuteSaveJob(
            SaveJob job,
            CancellationToken cancellationToken = default,
            IProgress<SaveState>? progress = null,
            PriorityFileService? priorityFileService = null,
            LargeFileTransferService? largeFileTransferService = null)
        {
            var settings = _settingsService.LoadSettings();

            var allFiles = Directory.GetFiles(job.SourceFolder, "*", SearchOption.AllDirectories)
                .ToList();

            if (priorityFileService != null)
            {
                foreach (var file in allFiles)
                {
                    priorityFileService.AddPendingFile(file);
                }

                allFiles = allFiles
                    .OrderByDescending(file => priorityFileService.IsPriorityFile(file))
                    .ToList();
            }

            int totalFiles = allFiles.Count;
            long totalBytes = allFiles.Sum(f => new FileInfo(f).Length);
            int remaining = totalFiles;
            long remainingBytes = totalBytes;

            foreach (var sourceFile in allFiles)
            {
                string relativePath = Path.GetRelativePath(job.SourceFolder, sourceFile);
                string targetFile = Path.Combine(job.TargetFolder, relativePath);
                long fileSize = new FileInfo(sourceFile).Length;

                Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);

                var state = new SaveState
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
                };

                _stateService.UpdateState(state);
                progress?.Report(state);

                long transferMs = -1;
                long encryptionTimeMs = 0;
                string error = string.Empty;

                try
                {
                    var sw = Stopwatch.StartNew();

                    if (largeFileTransferService != null)
                    {
                        largeFileTransferService
                            .ExecuteTransferAsync(
                                sourceFile,
                                () =>
                                {
                                    File.Copy(sourceFile, targetFile, overwrite: true);
                                    return Task.CompletedTask;
                                },
                                cancellationToken)
                            .GetAwaiter()
                            .GetResult();
                    }
                    else
                    {
                        File.Copy(sourceFile, targetFile, overwrite: true);
                    }

                    sw.Stop();
                    transferMs = sw.ElapsedMilliseconds;

                    if (_cryptoService.NeedsEncryption(targetFile, settings.EncryptedExtensions))
                    {
                        encryptionTimeMs = _cryptoService.Encrypt(targetFile, settings.CryptoSoftPath);
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
        }
    }
}