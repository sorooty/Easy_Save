using EasySave.Core.Model.Entities;

namespace EasySave.Core.Model.Strategies
{
    /// <summary>
    /// Contrat commun à toutes les stratégies de sauvegarde.
    /// Permet à SaveExecutor de déléguer la logique de copie sans connaître le type concret.
    /// </summary>
    public interface ISaveStrategy
    {
        void ExecuteSaveJob(SaveJob job);
    }
}
