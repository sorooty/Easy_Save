namespace EasySave.Core.Model.Entities
{
    /// <summary>
    /// Instantané de l'état d'un travail de sauvegarde à un instant T.
    /// Écrit en temps réel dans state.json par StateService.
    /// </summary>
    public class SaveState
    {
        public string Name { get; set; } = string.Empty;

        // Horodatage de la dernière action (format "yyyy-MM-dd HH:mm:ss")
        public string LastActionTime { get; set; } = string.Empty;

        // "Active", "Completed", "Error", "Idle"
        public string Status { get; set; } = string.Empty;

        public int TotalFiles { get; set; }
        public long TotalSizeBytes { get; set; }
        public int RemainingFiles { get; set; }
        public long RemainingFilesBytes { get; set; }
        public int ProgressPercent { get; set; }

        // Chemins du fichier en cours de transfert
        public string CurrentSourceFile { get; set; } = string.Empty;
        public string CurrentTargetFile { get; set; } = string.Empty;
    }
}
