namespace EasyLog;

public class LogEntry
{
    // Les attributs qui contient les contenus de log
    public DateTime TimeStamp { get; set; }
    public string JobName { get; set; }
    public string SourceFile { get; set; }
    public string TargetFile { get; set; }
    public long FileSizeBytes { get; set; }
    public long TransferDurationMs { get; set; }
    public string State { get; set; }
    public string ErrorMessage { get; set; }
    public long EncryptionTimeMs { get; set; }

    /// <summary>Parameterless constructor required for JSON/XML deserialization.</summary>
    public LogEntry()
    {
        JobName = string.Empty;
        SourceFile = string.Empty;
        TargetFile = string.Empty;
        State = string.Empty;
        ErrorMessage = string.Empty;
    }

    // Initialiser les attributs
    public LogEntry(
        string jobName,
        string sourceFile,
        string targetFile,
        long fileSizeBytes,
        long transferDuration,
        string state,
        string errorMessage,
        long encryptionTimeMs = 0
    ) 
    {
        TimeStamp = DateTime.Now;
        JobName = jobName;
        SourceFile = sourceFile;
        TargetFile = targetFile;
        FileSizeBytes = fileSizeBytes;
        TransferDurationMs = transferDuration;
        State = state;
        ErrorMessage = errorMessage;
        EncryptionTimeMs = encryptionTimeMs;
    }
}