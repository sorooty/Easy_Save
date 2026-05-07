using System.Xml.Serialization;

namespace EasyLog;

public class XmlLogger : ILogger
{
    private readonly string _logDirectory;
    private readonly string _logFilePath;

    /// <summary>
    /// Création le cheimin pour enregistrer les logs dans un fichier XML.
    /// Si le dossier n'existe pas, il sera créé automatiquement.
    /// </summary>
    /// <param name="entry">
    /// L'objet <see cref="LogEntry"/> représentant les informations du log à enregistrer.
    /// </param>
    /// <remarks>
    /// les logs sont stockés sous forme de liste dans un fichier XML.
    /// Chaque appel à cette méthode ajoute une nouvelle entrée à la liste existante
    /// </remarks>
    /// <exception cref="IOException">
    /// Peut être levée en cas de problème d'accès au fichier.
    /// </exception>
    /// <exception cref="JsonException">
    /// Peut être levée si le contenu XML existant est invalide. 
    /// </exception>
    public XmlLogger(string logDirectory)
    {
        _logDirectory = logDirectory;
        _logFilePath = Path.Combine(_logDirectory, "log.xml");

        Console.WriteLine($"LOG FILE PATH: {_logFilePath}");
        
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    /// <summary>
    /// Enregistre une entrée de log dans un fichier XML.
    /// Sinon, un noveau fichier est créé.
    /// </summary>
    /// <param name="entry">
    /// 
    /// </param>


    public void Log(LogEntry entry)
    {
        List<LogEntry> logs;

        // Si le fichier existe --> lire le log ancien
        if (File.Exists(_logFilePath))
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));

                using (FileStream fs = new FileStream(_logFilePath, FileMode.Open))
                {
                    logs = serializer.Deserialize(fs) as List<LogEntry> ?? new List<LogEntry>();
                }
            }
            catch (Exception ex)
            {
                // Si le contenu XML est invalide, on initialise une nouvelle liste de logs
                Console.WriteLine($"Error reading log file: {ex.Message}");
                logs = new List<LogEntry>();
            }

        }
        else
        {
            logs = new List<LogEntry>();
        }

        // Ajouter la nouvelle entrée de log à la liste
        logs.Add(entry);

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<LogEntry>));

        using (FileStream fs = new FileStream(_logFilePath, FileMode.Create))
        {
            xmlSerializer.Serialize(fs, logs);
        }
    }
}