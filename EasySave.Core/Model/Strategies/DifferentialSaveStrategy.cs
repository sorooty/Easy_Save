using System.Diagnostics;
using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;

namespace EasySave.Core.Model.Strategies
{
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

        public void ExecuteSaveJob(
            SaveJob job,
            CancellationToken cancellationToken = default,
            IProgress<SaveState>? progress = null,
            PriorityFileService? priorityFileService = null,
            LargeFileTransferService? largeFileTransferService = null,
            ManualResetEventSlim? pauseGate = null)
        {
            var settings = _settingsService.LoadSettings();
            var allFiles = Directory.GetFiles(job.SourceFolder, "*", SearchOption.AllDirectories);

            var filesToCopy = allFiles
                .Where(src => NeedsCopy(src, job))
                .ToList();

            if (priorityFileService != null)
            {
                foreach (var file in filesToCopy)
                {
                    priorityFileService.AddPendingFile(file);
                }

                filesToCopy = filesToCopy
                    .OrderByDescending(file => priorityFileService.IsPriorityFile(file))
                    .ToList();
            }

            int totalFiles = filesToCopy.Count;
            long totalBytes = filesToCopy.Sum(f => new FileInfo(f).Length);
            int remaining = totalFiles;
            long remainingBytes = totalBytes;

            foreach (var sourceFile in filesToCopy)
            {
                // Pause gate: block here (after completing the previous file) until resumed
                if (pauseGate != null && !pauseGate.IsSet)
                {
                    var pausedState = new SaveState
                    {
                        Name = job.Name,
                        Status = "Paused",
                        LastActionTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        TotalFiles = totalFiles,
                        TotalSizeBytes = totalBytes,
                        RemainingFiles = remaining,
                        RemainingFilesBytes = remainingBytes,
                        ProgressPercent = totalFiles == 0 ? 100 : (int)((totalFiles - remaining) * 100.0 / totalFiles)
                    };
                    _stateService.UpdateState(pausedState);
                    progress?.Report(pausedState);
                    pauseGate.Wait(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                }

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

                // Block non-priority files while any priority file is pending (across all jobs)
                if (priorityFileService != null && !priorityFileService.IsPriorityFile(sourceFile))
                    priorityFileService.WaitForNonPriority(cancellationToken);

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

                _logger.Log(new LogEntry(job.Name, sourceFile, targetFile, fileSize, transferMs,
                    state: error == string.Empty ? "OK" : "ERROR",
                    errorMessage: error,
                    encryptionTimeMs: encryptionTimeMs));

                priorityFileService?.RemovePendingFile(sourceFile);

                remaining--;
                remainingBytes -= fileSize;

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.Log(new LogEntry(job.Name, string.Empty, string.Empty, 0, -1,
                        state: "STOPPED", errorMessage: "Job interrupted: business software detected"));
                    return;
                }
            }
        }

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