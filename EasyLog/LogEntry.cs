namespace EasyLog
{
    /// <summary>
    /// Représente une entrée de log immuable pour un transfert de fichier.
    /// Toutes les propriétés sont en lecture seule — l'entrée est créée une seule fois.
    /// </summary>
    public class LogEntry
    {
        public DateTime Timestamp { get; }
        public string JobName { get; }
        public string SourceFile { get; }
        public string TargetFile { get; }
        public long FileSizeBytes { get; }

        // Durée du transfert en ms. Valeur négative = erreur lors de la copie.
        public long TransferDurationMs { get; }

        // Message d'erreur éventuel. Chaîne vide si le transfert a réussi.
        public string ErrorMessage { get; }

        public LogEntry(
            string jobName,
            string sourceFile,
            string targetFile,
            long fileSizeBytes,
            long transferDurationMs,
            string errorMessage = "")
        {
            Timestamp = DateTime.Now;
            JobName = jobName;
            SourceFile = sourceFile;
            TargetFile = targetFile;
            FileSizeBytes = fileSizeBytes;
            TransferDurationMs = transferDurationMs;
            ErrorMessage = errorMessage;
        }
    }
}
