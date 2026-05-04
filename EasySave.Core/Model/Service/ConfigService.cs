using System.Text.Json;
using EasySave.Core.Model.Entities;

namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Charge et persiste la liste des travaux de sauvegarde dans jobs.json.
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
    }
}
