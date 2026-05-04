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

        public FullSaveStrategy(ILogger logger, IStateService stateService)
        {
            _logger = logger;
            _stateService = stateService;
        }

        public void ExecuteSaveJob(SaveJob job)
        {
            // Collecte tous les fichiers source (récursif) avant de démarrer
            var allFiles = Directory.GetFiles(job.SourceFolder, "*", SearchOption.AllDirectories);
            int totalFiles = allFiles.Length;
            long totalBytes = allFiles.Sum(f => new FileInfo(f).Length);
            int remaining = totalFiles;
            long remainingBytes = totalBytes;

            foreach (var sourceFile in allFiles)
            {
                // Reconstitution du chemin de destination en préservant la structure de sous-dossiers
                string relativePath = Path.GetRelativePath(job.SourceFolder, sourceFile);
                string targetFile = Path.Combine(job.TargetFolder, relativePath);
                long fileSize = new FileInfo(sourceFile).Length;

                Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);

                // Mise à jour de l'état avant la copie pour un suivi en temps réel
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
                string error = string.Empty;

                try
                {
                    var sw = Stopwatch.StartNew();
                    File.Copy(sourceFile, targetFile, overwrite: true);
                    sw.Stop();
                    transferMs = sw.ElapsedMilliseconds;
                }
                catch (Exception ex)
                {
                    // Durée négative signale une erreur dans le log (convention projet)
                    error = ex.Message;
                }

                _logger.Log(new LogEntry(job.Name, sourceFile, targetFile, fileSize, transferMs, error));

                remaining--;
                remainingBytes -= fileSize;
            }
        }
    }
}
