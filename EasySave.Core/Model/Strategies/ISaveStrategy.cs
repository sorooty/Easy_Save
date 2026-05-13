using EasySave.Core.Model.Entities;

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
        /// </summary>
        void ExecuteSaveJob(SaveJob job, CancellationToken cancellationToken = default);
    }
}
