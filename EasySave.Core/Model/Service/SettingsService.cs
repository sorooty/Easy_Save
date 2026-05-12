using System.Text.Json;
using EasySave.Core.Model.Entities;

namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Charge et persiste les paramètres généraux (v2.0) dans settings.json.
    /// Superset rétrocompatible avec AppSettings de ConfigService.
    /// </summary>
    public class SettingsService
    {
        private readonly IPathService _paths;
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public SettingsService(IPathService paths)
        {
            _paths = paths;
        }

        public GeneralSettings LoadSettings()
        {
            if (!File.Exists(_paths.SettingsFile))
                return new GeneralSettings();

            try
            {
                var json = File.ReadAllText(_paths.SettingsFile);
                return JsonSerializer.Deserialize<GeneralSettings>(json) ?? new GeneralSettings();
            }
            catch
            {
                return new GeneralSettings();
            }
        }

        public void SaveSettings(GeneralSettings settings)
        {
            File.WriteAllText(_paths.SettingsFile, JsonSerializer.Serialize(settings, JsonOptions));
        }
    }
}
