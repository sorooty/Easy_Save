using System.IO;
using System.Text.Json;

namespace EasyLog;
public class JsonLogger : ILogger
{
    private readonly string _logDirectory;
    private readonly string _logFilePath;

    // Contructeur : Initialiser le chemin pour sauvegarder le log
    public JsonLogger(string logDirectory)
    {
        _logDirectory = logDirectory;
        _logFilePath = Path.Combine(_logDirectory, "log.json");

        // si le dossier n'existe pas. 
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);        // Création d'une nouvelle dossier pour enregistrer les fichiers logs
        }

    }   

    /// <summary>
    /// Enregistre une entrée de log dans un fichier JSON.
    /// Si le fichier existe déjà, les logs existants sont lui puis mis à jours.
    /// Sinon, un nouveau fichier est créé.
    /// </summary>
    /// <param name="entry">
    /// L'objet <see cref="LogEntry"/> représentant les informations du log à enregistrer.
    /// </param>
    /// <remarks>
    /// les logs sont stockés sous forme de liste dans un fichier JSON.
    /// Chaque appel à cette méthode ajoute une nouvelle entrée à la liste existante
    /// </remarks>
    /// <exception cref="IOException">
    /// Peut être levée en cas de problème d'accès au fichier.
    /// </exception>
    /// <exception cref="JsonException">
    /// Peut être levée si le contenu JSON existant est invalide. 
    /// </exception>
    public void Log(LogEntry entry)
    {
        List<LogEntry> logs;

        // Si le fichier existe --> lire le log ancien
        if (File.Exists(_logFilePath))
        {
            string json = File.ReadAllText(_logFilePath);
            logs = string.IsNullOrWhiteSpace(json) 
                ? new List<LogEntry>() 
                : JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
        }
        else
        {
            logs = new List<LogEntry>();
        }

        // Ajouter le nouveau log
        logs.Add(entry);

        string newJson = JsonSerializer.Serialize(logs, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_logFilePath, newJson);

    }
    
}

