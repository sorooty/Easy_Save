using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;

namespace EasySave.Core.Model.Strategies
{
    /// <summary>
    /// Contrat commun à toutes les stratégies de sauvegarde.
    /// Permet à SaveExecutor de déléguer la logique de copie sans connaître le type concret.
    /// </summary>
    public interface ISaveStrategy
    {
        /// <summary>
        /// Executes the backup job. Checks <paramref name="cancellationToken"/> after each file
        /// so it can stop gracefully (finishing the current file) if cancelled.
        /// If <paramref name="pauseGate"/> is provided and not set, the strategy blocks at the
        /// start of the next file until the gate is opened again (Resume).
        /// </summary>
        void ExecuteSaveJob(
        SaveJob job,
        CancellationToken cancellationToken = default,
        IProgress<SaveState>? progress = null,
        PriorityFileService? priorityFileService = null,
        LargeFileTransferService? largeFileTransferService = null,
        ManualResetEventSlim? pauseGate = null);
    }
}
