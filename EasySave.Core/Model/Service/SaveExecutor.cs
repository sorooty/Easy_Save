using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Strategies;

namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Orchestre l'exécution des travaux de sauvegarde.
    /// Délègue la logique de copie à la stratégie injectée (Full ou Differential).
    /// Pause and stop are driven by the <paramref name="pauseGate"/> and
    /// <paramref name="cancellationToken"/> provided by the caller.
    /// Business-software detection is handled externally by <see cref="BusinessAppWatcher"/>.
    /// </summary>
    public class SaveExecutor
    {
        private readonly ISaveStrategy _fullStrategy;
        private readonly ISaveStrategy _differentialStrategy;
        private readonly ILogger _logger;
        private readonly IStateService _stateService;
        private readonly PriorityFileService _priorityFileService;
        private readonly LargeFileTransferService _largeFileTransferService;

        public SaveExecutor(
        ISaveStrategy fullStrategy,
        ISaveStrategy differentialStrategy,
        ILogger logger,
        IStateService stateService,
        PriorityFileService priorityFileService,
        LargeFileTransferService largeFileTransferService)
            {
                _fullStrategy = fullStrategy;
                _differentialStrategy = differentialStrategy;
                _logger = logger;
                _stateService = stateService;
                _priorityFileService = priorityFileService;
                _largeFileTransferService = largeFileTransferService;
            }

        /// <summary>
        /// Exécute un seul travail de sauvegarde de façon asynchrone.
        /// Sélectionne la stratégie (Full ou Differential) selon <see cref="SaveJob.Type"/>.
        /// </summary>
        /// <returns>
        /// <c>false</c> if the job was blocked at launch by the business software;
        /// <c>true</c> if the job ran (completed or stopped mid-run).
        /// </returns>
        public async Task<bool> ExecuteAsync(
            SaveJob job,
            IProgress<SaveState>? progress,
            CancellationToken cancellationToken,
            ManualResetEventSlim? pauseGate = null)
        {
            var strategy = job.Type == SaveType.Full ? _fullStrategy : _differentialStrategy;

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                bool succeeded = false;
                try
                {
                    strategy.ExecuteSaveJob(job, cancellationToken, progress, _priorityFileService, _largeFileTransferService, pauseGate);
                    succeeded = !cancellationToken.IsCancellationRequested;
                }
                finally
                {
                    var finalState = new SaveState
                    {
                        Name = job.Name,
                        Status = succeeded ? "Completed" : (cancellationToken.IsCancellationRequested ? "Stopped" : "Error"),
                        LastActionTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        RemainingFiles = 0,
                        ProgressPercent = succeeded ? 100 : 0
                    };
                    _stateService.UpdateState(finalState);
                    progress?.Report(finalState);
                }

            }, cancellationToken);

            return true;
        }

        /// <summary>
        /// Exécute tous les travaux en parallèle.
        /// Chaque job tourne sur son propre Task. Les verrous dans StateService, JsonLogger
        /// et XmlLogger garantissent l'absence de corruption des fichiers partagés.
        /// </summary>
        public async Task ExecuteAllAsync(
            List<SaveJob> jobs,
            IProgress<SaveState>? progress,
            CancellationToken cancellationToken)
        {
            var tasks = jobs.Select(job =>
                ExecuteAsync(job, progress, cancellationToken));

            await Task.WhenAll(tasks);
        }
    }
}
