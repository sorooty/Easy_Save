using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;

namespace EasySave.ViewModel;

/// <summary>
/// ViewModel pour la vue Paramètres (log format, langue, extensions, logiciel métier, CryptoSoft).
/// </summary>
public class SettingsViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private readonly LanguageService _languageService;

    private bool _useJson = true;
    private bool _useXml;
    private bool _useFrench;
    private bool _useEnglish = true;
    private string _encryptedExtensions = string.Empty;
    private string _businessSoftwareName = string.Empty;
    private string _cryptoSoftPath = string.Empty;
    private string _savedMessage = string.Empty;
    private string _initialLanguage = "en";

    /// <summary>
    /// Appelé par App.xaml.cs pour déclencher le redémarrage de l'application
    /// lorsque la langue change (les labels {x:Static} ne sont pas dynamiques).
    /// </summary>
    public Action? RequestRestart { get; set; }

    #region Propriétés

    public bool UseJson
    {
        get => _useJson;
        set
        {
            Set(ref _useJson, value);
            if (value) UseXml = false;
        }
    }

    public bool UseXml
    {
        get => _useXml;
        set
        {
            Set(ref _useXml, value);
            if (value) UseJson = false;
        }
    }

    public bool UseFrench
    {
        get => _useFrench;
        set
        {
            Set(ref _useFrench, value);
            if (value) UseEnglish = false;
        }
    }

    public bool UseEnglish
    {
        get => _useEnglish;
        set
        {
            Set(ref _useEnglish, value);
            if (value) UseFrench = false;
        }
    }

    /// <summary>Extensions chiffrées sous forme de chaîne séparée par des virgules (ex : ".docx, .xlsx").</summary>
    public string EncryptedExtensions
    {
        get => _encryptedExtensions;
        set => Set(ref _encryptedExtensions, value);
    }

    public string BusinessSoftwareName
    {
        get => _businessSoftwareName;
        set => Set(ref _businessSoftwareName, value);
    }

    public string CryptoSoftPath
    {
        get => _cryptoSoftPath;
        set => Set(ref _cryptoSoftPath, value);
    }

    public string SavedMessage
    {
        get => _savedMessage;
        private set => Set(ref _savedMessage, value);
    }

    public bool HasSavedMessage => !string.IsNullOrEmpty(_savedMessage);

    #endregion

    public RelayCommand SaveCommand { get; }

    public SettingsViewModel(SettingsService settingsService, LanguageService languageService)
    {
        _settingsService = settingsService;
        _languageService = languageService;

        SaveCommand = new RelayCommand(_ => Save());

        Load();
        _initialLanguage = UseFrench ? "fr" : "en";
    }

    private void Load()
    {
        var s = _settingsService.LoadSettings();

        UseJson = s.LogFormat == EasyLog.LogFormat.JSON;
        _useXml = !_useJson;
        OnPropertyChanged(nameof(UseXml));

        UseEnglish = s.Language == "en";
        _useFrench = !_useEnglish;
        OnPropertyChanged(nameof(UseFrench));

        EncryptedExtensions = string.Join(", ", s.EncryptedExtensions ?? new List<string>());
        BusinessSoftwareName = s.BusinessSoftwareName;
        CryptoSoftPath = s.CryptoSoftPath;
    }

    private void Save()
    {
        var newLanguage = UseFrench ? "fr" : "en";
        var languageChanged = newLanguage != _initialLanguage;

        var s = new GeneralSettings
        {
            LogFormat = UseXml ? EasyLog.LogFormat.XML : EasyLog.LogFormat.JSON,
            Language = newLanguage,
            BusinessSoftwareName = BusinessSoftwareName,
            CryptoSoftPath = CryptoSoftPath,
            EncryptedExtensions = EncryptedExtensions
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e))
                .ToList()
        };

        _settingsService.SaveSettings(s);
        _languageService.SetLanguage(s.Language);
        _initialLanguage = newLanguage;

        if (languageChanged)
        {
            // {x:Static} labels are evaluated once at XAML load — a restart is required
            // to apply the new culture. We show a brief message then trigger the restart.
            SavedMessage = _languageService.GetText("settings.restarting");
            OnPropertyChanged(nameof(HasSavedMessage));
            Task.Delay(900).ContinueWith(
                _ => RequestRestart?.Invoke(),
                System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            return;
        }

        SavedMessage = _languageService.GetText("settings.saved");
        OnPropertyChanged(nameof(HasSavedMessage));
        Task.Delay(2000).ContinueWith(_ =>
        {
            SavedMessage = string.Empty;
            OnPropertyChanged(nameof(HasSavedMessage));
        }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
    }
}
