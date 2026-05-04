namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Implémentation des chemins réels de l'application.
    /// Tous les fichiers sont stockés dans %AppData%\EasySave pour respecter la convention
    /// "pas de chemins en dur type C:\temp\" imposée par ProSoft.
    /// </summary>
    public class AppPaths : IPathService
    {
        // Répertoire racine de l'application dans le profil utilisateur
        private static readonly string BaseDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EasySave");

        public string JobsFile => Path.Combine(BaseDir, "jobs.json");
        public string StateFile => Path.Combine(BaseDir, "state.json");
        public string LogsDirectory => Path.Combine(BaseDir, "Logs");

        public void EnsureDirectoriesExist()
        {
            Directory.CreateDirectory(BaseDir);
            Directory.CreateDirectory(LogsDirectory);
        }
    }
}
