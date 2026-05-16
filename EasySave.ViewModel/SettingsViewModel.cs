using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using System.Collections.Generic;

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
    private string _priorityExtensions = string.Empty;
    private long _largeFileLimitKo;
    private EasyLog.LogStorageMode _logStorageMode = EasyLog.LogStorageMode.LocalOnly;
    private string _centralLoggingEndpoint = string.Empty;

    public long LargeFileLimitKo
    {
        get => _largeFileLimitKo;
        set { _largeFileLimitKo = value; OnPropertyChanged(); }
    }

    public IEnumerable<EasyLog.LogStorageMode> LogStorageModeValues =>
        Enum.GetValues<EasyLog.LogStorageMode>();

    public EasyLog.LogStorageMode LogStorageMode
    {
        get => _logStorageMode;
        set => Set(ref _logStorageMode, value);
    }

    public string CentralLoggingEndpoint
    {
        get => _centralLoggingEndpoint;
        set => Set(ref _centralLoggingEndpoint, value);
    }
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

    public string PriorityExtensions
    {
        get => _priorityExtensions;
        set => Set(ref _priorityExtensions, value);
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
    public RelayCommand OpenLogsFolderCommand { get; }

    /// <summary>Injected by App.xaml.cs — opens the logs directory in Explorer.</summary>
    public Action? OpenLogsFolder { get; set; }

    public SettingsViewModel(SettingsService settingsService, LanguageService languageService)
    {
        _settingsService = settingsService;
        _languageService = languageService;

        SaveCommand           = new RelayCommand(_ => Save());
        OpenLogsFolderCommand = new RelayCommand(_ => OpenLogsFolder?.Invoke());

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
        PriorityExtensions = string.Join(", ", s.PriorityExtensions ?? new List<string>());
        BusinessSoftwareName = s.BusinessSoftwareName;
        CryptoSoftPath = s.CryptoSoftPath;
        LargeFileLimitKo = s.LargeFileLimitKo;
        LogStorageMode = s.LogStorageMode;
        CentralLoggingEndpoint = s.CentralLoggingEndpoint;
    }

    private async void Save()
    {
        var newLanguage = UseFrench ? "fr" : "en";
        var languageChanged = newLanguage != _initialLanguage;

        var s = new GeneralSettings
        {
            LogFormat = UseXml ? EasyLog.LogFormat.XML : EasyLog.LogFormat.JSON,
            Language = newLanguage,
            BusinessSoftwareName = BusinessSoftwareName,
            CryptoSoftPath = CryptoSoftPath,
            LargeFileLimitKo = LargeFileLimitKo,

            EncryptedExtensions = EncryptedExtensions
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(e => e.Trim())
        .Where(e => !string.IsNullOrEmpty(e))
        .ToList(),

            PriorityExtensions = PriorityExtensions
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(e => e.Trim())
        .Where(e => !string.IsNullOrEmpty(e))
        .ToList(),

            LogStorageMode = LogStorageMode,
            CentralLoggingEndpoint = CentralLoggingEndpoint
        };

        _settingsService.SaveSettings(s);
        _languageService.SetLanguage(s.Language);
        _initialLanguage = newLanguage;

        if (languageChanged)
        {
            SavedMessage = _languageService.GetText("settings.restarting");
            OnPropertyChanged(nameof(HasSavedMessage));

            await Task.Delay(900);
            RequestRestart?.Invoke();
            return;
        }

        SavedMessage = _languageService.GetText("settings.saved");
        OnPropertyChanged(nameof(HasSavedMessage));

        await Task.Delay(2000);

        SavedMessage = string.Empty;
        OnPropertyChanged(nameof(HasSavedMessage));
    }


}
