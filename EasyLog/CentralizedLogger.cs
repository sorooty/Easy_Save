using System.Net.Http.Json;

namespace EasyLog;

/// <summary>
/// Envoie de manière asynchrone des entrées de journalisation (LogEntry) vers un point de terminaison central via HTTP.
/// </summary>
/// <remarks>Crée un HttpClient privé pour les requêtes HTTP. Pour une utilisation à grande échelle, privilégier
/// l'injection d'un HttpClient ou IHttpClientFactory afin d'éviter l'épuisement des sockets. La méthode WriteLog
/// capture les exceptions et les consigne sur la console ; les échecs d'envoi ne sont pas propagés. Envisager d'ajouter
/// une stratégie de retry/backoff, une mise en file locale ou une validation de l'URL de destination.</remarks>
public class CentralizedLogger
{
    private readonly HttpClient _httpClient;
    private readonly string _centralLoggingEndpoint;
    public CentralizedLogger(string centralLoggingEndpoint)
    {
        _httpClient = new HttpClient();
        _centralLoggingEndpoint = centralLoggingEndpoint;
    }

    /// <summary>
    /// Envoie de manière asynchrone une entrée de journal à un point de terminaison de journalisation central.
    /// </summary>
    /// <remarks>Les erreurs d'envoi sont capturées et traitées localement. Envisager une stratégie de retry
    /// ou une persistance locale pour garantir la livraison.</remarks>
    /// <param name="logEntry">Entrée de journal contenant les informations (message, niveau, contexte) à transmettre.</param>
    /// <returns>Tâche représentant l'opération asynchrone d'envoi ; les exceptions sont gérées en interne.</returns>
    public async Task WriteLog(LogEntry logEntry)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_centralLoggingEndpoint, logEntry);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log to local file, retry logic, etc.)
            Console.WriteLine($"Failed to send log entry to central server: {ex.Message}");
        }
    }
}
