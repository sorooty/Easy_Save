using System.Xml.Serialization;

namespace EasyLog;

public class XmlLogger : ILogger
{
    private readonly string _logDirectory;
    private readonly string _logFilePath;
    private readonly object _lock = new();

    /// <summary>
    /// Création le cheimin pour enregistrer les logs dans un fichier XML.
    /// Si le dossier n'existe pas, il sera créé automatiquement.
    /// </summary>
    /// <param name="logDirectory">
    /// Le chemin du dossier où les logs seront enregistrés.
    /// </param>
    /// <remarks>
    /// Les logs sont stockés sous forme de liste dans un fichier XML.
    /// Chaque appel à cette méthode ajoute une nouvelle entrée à la liste existante.
    /// Si le répertoire spécifié n'existe pas, il est créé automatiquement. Le fichier de journal sera nommé "YYYY-MM-DD.xml" dans ce répertoire.
    /// </remarks>
    public XmlLogger(string logDirectory)
    {
        _logDirectory = logDirectory;
        _logFilePath = Path.Combine(_logDirectory, $"{DateTime.Now:yyyy-MM-dd}.xml");
        
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
    /// L'objet <see cref="LogEntry"/> représentant les informations du log à enregistrer.
    /// </param>
    /// <remarks>
    /// les logs sont stockés sous forme de liste dans un fichier XML.
    /// Chaque appel à cette méthode ajoute une nouvelle entrée à la liste existante.   
    /// </remarks>
    /// <exception cref="IOException">
    /// Peut être levée en cas de problème d'accès au fichier.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Peut être levée si le contenu XML existant est invalide. 
    /// </exception>


    public void Log(LogEntry entry)
    {
        lock (_lock)
        {
        List<LogEntry> logs;
        XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));

        if (File.Exists(_logFilePath))
        {
            try
            {
                using (FileStream fs = new FileStream(_logFilePath, FileMode.Open))
                {
                    logs = serializer.Deserialize(fs) as List<LogEntry> ?? new List<LogEntry>();
                }
            }
            catch (InvalidOperationException)
            {
                logs = new List<LogEntry>();
            }
        }
        else
        {
            logs = new List<LogEntry>();
        }

        logs.Add(entry);

        using (FileStream fs = new FileStream(_logFilePath, FileMode.Create))
        {
            serializer.Serialize(fs, logs);
        }
        }
    }
}