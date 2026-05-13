using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Strategies;

namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Orchestre l'exécution des travaux de sauvegarde.
    /// Délègue la logique de copie à la stratégie injectée (Full ou Differential).
    /// An optional <see cref="IsBlocked"/> delegate is polled during execution:
    /// if it returns true the current file is completed then the job is cancelled.
    /// </summary>
    public class SaveExecutor
    {
        private readonly ISaveStrategy _fullStrategy;
        private readonly ISaveStrategy _differentialStrategy;
        private readonly ILogger _logger;
        private readonly IStateService _stateService;

        /// <summary>
        /// Optional predicate checked every 500 ms during execution.
        /// Return true to stop after the current file (business software detection).
        /// </summary>
        public Func<bool>? IsBlocked { get; set; }

        public SaveExecutor(ISaveStrategy fullStrategy, ISaveStrategy differentialStrategy, ILogger logger, IStateService stateService)
        {
            _fullStrategy = fullStrategy;
            _differentialStrategy = differentialStrategy;
            _logger = logger;
            _stateService = stateService;
        }

        /// <summary>
        /// Exécute un seul travail de sauvegarde de façon asynchrone.
        /// Sélectionne la stratégie (Full ou Differential) selon <see cref="SaveJob.Type"/>.
        /// </summary>
        public async Task ExecuteAsync(
            SaveJob job,
            IProgress<SaveState>? progress,
            CancellationToken cancellationToken)
        {
            var strategy = job.Type == SaveType.Full ? _fullStrategy : _differentialStrategy;

            // Linked token: cancelled by the caller OR by the business-software poller below
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Background poller: triggers cancellation if business software is detected
            Task? pollerTask = null;
            if (IsBlocked != null)
            {
                pollerTask = Task.Run(async () =>
                {
                    while (!linkedCts.Token.IsCancellationRequested)
                    {
                        if (IsBlocked())
                        {
                            linkedCts.Cancel();
                            return;
                        }
                        await Task.Delay(500, linkedCts.Token).ConfigureAwait(false);
                    }
                }, CancellationToken.None);
            }

            await Task.Run(() =>
            {
                linkedCts.Token.ThrowIfCancellationRequested();

                var initialState = new SaveState
                {
                    Name = job.Name,
                    Status = "Active",
                    LastActionTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                _stateService.UpdateState(initialState);
                progress?.Report(initialState);

                bool succeeded = false;
                try
                {
                    strategy.ExecuteSaveJob(job, linkedCts.Token);
                    succeeded = !linkedCts.IsCancellationRequested;
                }
                finally
                {
                    var finalState = new SaveState
                    {
                        Name = job.Name,
                        Status = succeeded ? "Completed" : (linkedCts.IsCancellationRequested ? "Stopped" : "Error"),
                        LastActionTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        RemainingFiles = 0,
                        ProgressPercent = succeeded ? 100 : 0
                    };
                    _stateService.UpdateState(finalState);
                    progress?.Report(finalState);
                }

            }, linkedCts.Token);

            // Stop the poller
            if (pollerTask != null)
            {
                linkedCts.Cancel();
                try { await pollerTask.ConfigureAwait(false); } catch (OperationCanceledException) { }
            }
        }

        /// <summary>
        /// Exécute tous les travaux séquentiellement.
        /// S'arrête immédiatement si le token d'annulation est déclenché.
        /// </summary>
        public async Task ExecuteAllAsync(
            List<SaveJob> jobs,
            IProgress<SaveState>? progress,
            CancellationToken cancellationToken)
        {
            foreach (var job in jobs)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ExecuteAsync(job, progress, cancellationToken);
            }
        }
    }
}
