using System.Diagnostics;

namespace EasySave.Core.Model.Service;

/// <summary>Manages Docker Compose lifecycle for the log server.</summary>
public class DockerService
{
    private readonly string _composeFilePath;

    public DockerService(string composeFilePath)
    {
        _composeFilePath = composeFilePath;
    }

    /// <summary>
    /// Starts the log server container if not already running.
    /// Returns true on success, false if docker is unavailable or the file does not exist.
    /// </summary>
    public async Task<bool> EnsureLogServerRunningAsync()
    {
        if (!File.Exists(_composeFilePath))
            return false;

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"compose -f \"{_composeFilePath}\" up -d",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            if (process == null) return false;
            await process.WaitForExitAsync();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Returns candidate paths where docker-compose.yml might exist relative to the app.</summary>
    public static IEnumerable<string> GetCandidatePaths()
    {
        var baseDir = AppContext.BaseDirectory;
        yield return Path.GetFullPath(Path.Combine(baseDir, "docker-compose.yml"));
        yield return Path.GetFullPath(Path.Combine(baseDir, "..", "docker-compose.yml"));
        yield return Path.GetFullPath(Path.Combine(baseDir, "..", "..", "docker-compose.yml"));
        yield return Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "docker-compose.yml"));
        yield return Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "docker-compose.yml"));
    }
}
