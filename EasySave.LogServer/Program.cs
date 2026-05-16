using EasyLog;
using System.Runtime.CompilerServices;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/logs", async (LogEntry logEntry) =>
{
    string logPath = "central_log.json";

    List<LogEntry> logs = new();

    if (File.Exists(logPath))
    {
        string existingJson = await File.ReadAllTextAsync(logPath);

        if(!string.IsNullOrWhiteSpace(existingJson))
        {
            logs = JsonSerializer.Deserialize<List<LogEntry>>(existingJson) ?? new List<LogEntry>();
        }
    }

    logs.Add(logEntry);

    string updatedJson = JsonSerializer.Serialize(
        logs, 
        new JsonSerializerOptions { 
            WriteIndented = true
        });

    await File.WriteAllTextAsync(logPath, updatedJson);

    Console.WriteLine($"Received log entry: {logEntry.JobName}");
    return Results.Ok();
});

app.Urls.Add("http://+:5275");
app.Run();