namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Contrat pour la résolution des chemins de fichiers de l'application.
    /// Permet de remplacer AppPaths par un mock lors des tests.
    /// </summary>
    public interface IPathService
    {
        string JobsFile { get; }
        string StateFile { get; }
        string LogsDirectory { get; }

        /// <summary>Crée les répertoires nécessaires s'ils n'existent pas encore.</summary>
        void EnsureDirectoriesExist();
    }
}
