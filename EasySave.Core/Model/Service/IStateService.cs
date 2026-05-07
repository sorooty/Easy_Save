using EasySave.Core.Model.Entities;

namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Contrat de mise à jour de l'état temps réel des travaux de sauvegarde.
    /// </summary>
    public interface IStateService
    {
        void UpdateState(SaveState state);
    }
}
