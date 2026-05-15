using System.Net.Http.Json;

namespace EasyLog;

public class CentralizedLogger
{
    private readonly HttpClient _httpClient;
    private readonly string _centralLoggingEndpoint;
    public CentralizedLogger(string centralLoggingEndpoint)
    {
        _httpClient = new HttpClient();
        _centralLoggingEndpoint = centralLoggingEndpoint;
    }
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
