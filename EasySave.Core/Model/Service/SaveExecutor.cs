using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Strategies;

namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Orchestre l'exécution des travaux de sauvegarde.
    /// Délègue la logique de copie à la stratégie injectée (Full ou Differential).
    /// </summary>
    public class SaveExecutor
    {
        private readonly ISaveStrategy _fullStrategy;
        private readonly ISaveStrategy _differentialStrategy;
        private readonly ILogger _logger;
        private readonly IStateService _stateService;

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

            // Exécution sur thread pool pour ne pas bloquer le thread appelant (UI ou console)
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                // État initial : travail actif
                var initialState = new SaveState
                {
                    Name = job.Name,
                    Status = "Active",
                    LastActionTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                _stateService.UpdateState(initialState);
                progress?.Report(initialState);

                try
                {
                    strategy.ExecuteSaveJob(job);
                }
                finally
                {
                    // État final toujours écrit, même en cas d'erreur partielle
                    var finalState = new SaveState
                    {
                        Name = job.Name,
                        Status = "Completed",
                        LastActionTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        RemainingFiles = 0,
                        ProgressPercent = 100
                    };
                    _stateService.UpdateState(finalState);
                    progress?.Report(finalState);
                }

            }, cancellationToken);
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
