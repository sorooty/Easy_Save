using EasyLog;

namespace EasySave.Core.Model.Entities
{
    /// <summary>
    /// Paramètres généraux de l'application (v2.0).
    /// Persisted in settings.json via SettingsService.
    /// </summary>
    public class GeneralSettings
    {
        public LogFormat LogFormat { get; set; } = LogFormat.JSON;

        /// <summary>Extensions de fichiers à chiffrer via CryptoSoft (ex : [".docx", ".xlsx"]).</summary>
        public List<string> EncryptedExtensions { get; set; } = new List<string>();

        /// <summary>Nom du processus métier bloquant (sans .exe, ex : "calc").</summary>
        public string BusinessSoftwareName { get; set; } = string.Empty;

        /// <summary>Chemin complet vers CryptoSoft.exe.</summary>
        public string CryptoSoftPath { get; set; } = string.Empty;

        /// <summary>Code langue actif ("en" ou "fr").</summary>
        public string Language { get; set; } = "en";
    }
}
