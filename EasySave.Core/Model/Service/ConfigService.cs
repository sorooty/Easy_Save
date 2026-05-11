using System.Text.Json;
using EasyLog;
using EasySave.Core.Model.Entities;

namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Charge et persiste la liste des travaux de sauvegarde dans jobs.json,
    /// et les paramètres de l'application (format de log) dans settings.json.
    /// </summary>
    public class ConfigService
    {
        private readonly IPathService _paths;

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public ConfigService(IPathService paths)
        {
            _paths = paths;
        }

        /// <summary>Retourne la liste des travaux depuis jobs.json. Liste vide si le fichier n'existe pas.</summary>
        public List<SaveJob> LoadJobs()
        {
            if (!File.Exists(_paths.JobsFile))
                return new List<SaveJob>();

            try
            {
                var json = File.ReadAllText(_paths.JobsFile);
                return JsonSerializer.Deserialize<List<SaveJob>>(json) ?? new List<SaveJob>();
            }
            catch
            {
                // Fichier corrompu : retourner liste vide sans crasher
                return new List<SaveJob>();
            }
        }

        /// <summary>Écrit la liste complète des travaux dans jobs.json (remplacement total).</summary>
        public void SaveJobs(List<SaveJob> jobs)
        {
            File.WriteAllText(_paths.JobsFile, JsonSerializer.Serialize(jobs, JsonOptions));
        }

        /// <summary>
        /// Retourne le format de log persisté dans settings.json.
        /// Retourne <see cref="LogFormat.JSON"/> par défaut (rétrocompatibilité v1.0).
        /// </summary>
        public LogFormat GetLogFormat()
        {
            if (!File.Exists(_paths.SettingsFile))
                return LogFormat.JSON;

            try
            {
                var json = File.ReadAllText(_paths.SettingsFile);
                var doc = JsonSerializer.Deserialize<AppSettings>(json);
                return doc?.LogFormat ?? LogFormat.JSON;
            }
            catch
            {
                return LogFormat.JSON;
            }
        }

        /// <summary>Persiste le format de log dans settings.json.</summary>
        public void SetLogFormat(LogFormat format)
        {
            var settings = new AppSettings { LogFormat = format };
            File.WriteAllText(_paths.SettingsFile, JsonSerializer.Serialize(settings, JsonOptions));
        }

        private class AppSettings
        {
            public LogFormat LogFormat { get; set; } = LogFormat.JSON;
        }
    }
}
