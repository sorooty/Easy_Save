namespace EasySave.Core.Model.Entities
{
    /// <summary>
    /// Représente la configuration d'un travail de sauvegarde.
    /// Persisté dans jobs.json via ConfigService.
    /// </summary>
    public class SaveJob
    {
        public string Name { get; set; } = string.Empty;
        public string SourceFolder { get; set; } = string.Empty;
        public string TargetFolder { get; set; } = string.Empty;
        public SaveType Type { get; set; } = SaveType.Full;
    }
}
