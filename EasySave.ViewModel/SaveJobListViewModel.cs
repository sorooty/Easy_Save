using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using System.Collections.ObjectModel;

namespace EasySave.ViewModel;

/// <summary>
/// ViewModel gérant la collection de jobs de sauvegarde.
/// Contient et orchestre les SaveJobViewModel.
/// </summary>
public class SaveJobListViewModel : ViewModelBase
{
    // Champs privés
    private readonly SaveExecutor _saveExecutor;
    private readonly ConfigService _configService;
    private readonly LanguageService _languageService;
    private readonly BusinessSoftwareService? _businessSoftwareService;
    private readonly SettingsService? _settingsService;

    // États UI
    private SaveJobViewModel? _selectedJob;
    private bool _isAddingJob;
    private bool _isRunningAll;
    private string _newJobName = string.Empty;
    private string _newJobSource = string.Empty;
    private string _newJobTarget = string.Empty;
    private SaveType _newJobType = SaveType.Full;
    private string _formError = string.Empty;

    #region Collections

    /// <summary>Collection observable des jobs (liée à l'interface).</summary>
    public ObservableCollection<SaveJobViewModel> Jobs { get; }

    #endregion

    #region Sélection et état

    public SaveJobViewModel? SelectedJob
    {
        get => _selectedJob;
        set
        {
            Set(ref _selectedJob, value);
            OnPropertyChanged(nameof(HasSelectedJob));
            OnPropertyChanged(nameof(ShowEmptyState));
            DeleteJobCommand.RaiseCanExecuteChanged();
        }
    }

    public bool HasSelectedJob => _selectedJob != null;
    public bool ShowEmptyState => !_isAddingJob && _selectedJob == null;

    public bool IsAddingJob
    {
        get => _isAddingJob;
        private set
        {
            Set(ref _isAddingJob, value);
            OnPropertyChanged(nameof(ShowEmptyState));
        }
    }

    public bool IsRunningAll
    {
        get => _isRunningAll;
        private set
        {
            Set(ref _isRunningAll, value);
            OnPropertyChanged(nameof(IsNotRunningAll));
            ExecuteAllCommand.RaiseCanExecuteChanged();
        }
    }

    public bool IsNotRunningAll => !_isRunningAll;

    #endregion

    #region Formulaire d'ajout

    public string NewJobName
    {
        get => _newJobName;
        set => Set(ref _newJobName, value);
    }

    public string NewJobSource
    {
        get => _newJobSource;
        set => Set(ref _newJobSource, value);
    }

    public string NewJobTarget
    {
        get => _newJobTarget;
        set => Set(ref _newJobTarget, value);
    }

    public bool IsFullType
    {
        get => _newJobType == SaveType.Full;
        set
        {
            if (value) _newJobType = SaveType.Full;
            OnPropertyChanged(nameof(IsFullType));
            OnPropertyChanged(nameof(IsDifferentialType));
        }
    }

    public bool IsDifferentialType
    {
        get => _newJobType == SaveType.Differential;
        set
        {
            if (value) _newJobType = SaveType.Differential;
            OnPropertyChanged(nameof(IsFullType));
            OnPropertyChanged(nameof(IsDifferentialType));
        }
    }

    public string FormError
    {
        get => _formError;
        private set
        {
            Set(ref _formError, value);
            OnPropertyChanged(nameof(HasFormError));
        }
    }

    public bool HasFormError => !string.IsNullOrEmpty(_formError);

    #endregion

    #region Commandes WPF

    public RelayCommand ShowAddFormCommand { get; }
    public RelayCommand CancelAddFormCommand { get; }
    public RelayCommand AddJobCommand { get; }
    public RelayCommand<SaveJobViewModel> DeleteJobCommand { get; }
    public RelayCommand ExecuteAllCommand { get; }
    public RelayCommand LoadJobsCommand { get; }

    #endregion

    /// <summary>
    /// Constructeur principal — les paramètres optionnels permettent la compat. console (v1.x).
    /// </summary>
    public SaveJobListViewModel(
        ConfigService configService,
        LanguageService languageService,
        SaveExecutor saveExecutor,
        BusinessSoftwareService? businessSoftwareService = null,
        SettingsService? settingsService = null)
    {
        _saveExecutor = saveExecutor;
        _configService = configService;
        _languageService = languageService;
        _businessSoftwareService = businessSoftwareService;
        _settingsService = settingsService;

        Jobs = new ObservableCollection<SaveJobViewModel>();

        ShowAddFormCommand = new RelayCommand(_ =>
        {
            IsAddingJob = true;
            _selectedJob = null;
            OnPropertyChanged(nameof(SelectedJob));
            OnPropertyChanged(nameof(HasSelectedJob));
            OnPropertyChanged(nameof(ShowEmptyState));
            ClearForm();
        });

        CancelAddFormCommand = new RelayCommand(_ =>
        {
            IsAddingJob = false;
            FormError = string.Empty;
        });

        AddJobCommand = new RelayCommand(_ => AddJobFromForm());

        DeleteJobCommand = new RelayCommand<SaveJobViewModel>(
            job => { if (job != null) RemoveJob(job); },
            job => job != null);

        ExecuteAllCommand = new RelayCommand(
            async _ => await RunExecuteAll(),
            _ => IsNotRunningAll);

        LoadJobsCommand = new RelayCommand(_ => LoadJobs());
    }

    #region Méthodes publiques (console + WPF)

    /// <summary>
    /// Ajoute un nouveau job à la liste (utilisé par la console et le formulaire WPF).
    /// </summary>
    public bool AddJob(string name, string sourceFolder, string targetFolder, string typeInput)
    {
        var newJob = new SaveJobViewModel(_saveExecutor, _languageService)
        {
            Name = name,
            SourceFolder = sourceFolder,
            TargetFolder = targetFolder,
            Type = typeInput == "1" ? SaveType.Full : SaveType.Differential
        };

        if (!newJob.IsValid())
            return false;

        if (Jobs.Any(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return false;

        Jobs.Add(newJob);
        newJob.EditConfirmed += SaveJobs;
        SaveJobs();
        return true;
    }

    /// <summary>Conservé pour compatibilité console — toujours false en v2.0 (illimité).</summary>
    public bool HasReachedMaxJobs() => false;

    /// <summary>Supprime un job par nom.</summary>
    public bool RemoveJobByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var job = Jobs.FirstOrDefault(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (job == null) return false;

        Jobs.Remove(job);
        SaveJobs();
        return true;
    }

    /// <summary>Exécute tous les jobs valides.</summary>
    public async Task ExecuteAll()
    {
        var validJobs = Jobs.ToList().Where(j => j.IsValid()).ToList();
        await Task.WhenAll(validJobs.Select(j => j.Execute()));
    }

    public async Task<bool> ExecuteJobs(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return false;

        var jobsToExecute = GetJobsFromCommand(command.Trim());
        if (jobsToExecute.Count == 0)
            return false;

        foreach (var job in jobsToExecute)
            await job.Execute();

        return true;
    }

    public void LoadJobs()
    {
        var savedJobs = _configService.LoadJobs();
        Jobs.Clear();
        foreach (var savedJob in savedJobs)
        {
            Jobs.Add(new SaveJobViewModel(_saveExecutor, _languageService)
            {
                Name = savedJob.Name,
                SourceFolder = savedJob.SourceFolder,
                TargetFolder = savedJob.TargetFolder,
                Type = savedJob.Type
            });
            Jobs[Jobs.Count - 1].EditConfirmed += SaveJobs;
        }
    }

    public void ChangeLanguage(string languageCode) => _languageService.SetLanguage(languageCode);

    public LogFormat GetLogFormat() => _configService.GetLogFormat();
    public void SetLogFormat(LogFormat format) => _configService.SetLogFormat(format);

    public string GetText(string key) => _languageService.GetText(key);

    public void SaveJobs()
    {
        _configService.SaveJobs(Jobs.Select(j => j.CreateJob()).ToList());
    }

    #endregion

    #region Méthodes privées WPF

    private void AddJobFromForm()
    {
        FormError = string.Empty;

        if (string.IsNullOrWhiteSpace(NewJobName))
        {
            FormError = _languageService.GetText("form.error.name_required");
            return;
        }

        if (string.IsNullOrWhiteSpace(NewJobSource))
        {
            FormError = _languageService.GetText("form.error.source_required");
            return;
        }

        if (string.IsNullOrWhiteSpace(NewJobTarget))
        {
            FormError = _languageService.GetText("form.error.target_required");
            return;
        }

        if (Jobs.Any(j => j.Name.Equals(NewJobName, StringComparison.OrdinalIgnoreCase)))
        {
            FormError = _languageService.GetText("form.error.name_exists");
            return;
        }

        // Vérifie le logiciel métier si configuré
        if (_businessSoftwareService != null && _settingsService != null)
        {
            var settings = _settingsService.LoadSettings();
            if (_businessSoftwareService.IsBusinessSoftwareRunning(settings.BusinessSoftwareName))
            {
                FormError = _languageService.GetText("job.blocked_by_business_app");
                return;
            }
        }

        var newJob = new SaveJobViewModel(_saveExecutor, _languageService)
        {
            Name = NewJobName,
            SourceFolder = NewJobSource,
            TargetFolder = NewJobTarget,
            Type = _newJobType
        };

        Jobs.Add(newJob);
        newJob.EditConfirmed += SaveJobs;
        SaveJobs();
        IsAddingJob = false;
        SelectedJob = newJob;
        ClearForm();
    }

    private void RemoveJob(SaveJobViewModel job)
    {
        bool wasSelected = SelectedJob == job;
        Jobs.Remove(job);
        SaveJobs();
        if (wasSelected) SelectedJob = null;
    }

    private async Task RunExecuteAll()
    {
        IsRunningAll = true;
        try
        {
            // Vérifie le logiciel métier
            if (_businessSoftwareService != null && _settingsService != null)
            {
                var settings = _settingsService.LoadSettings();
                if (_businessSoftwareService.IsBusinessSoftwareRunning(settings.BusinessSoftwareName))
                    return;
            }

            await ExecuteAll();
        }
        finally
        {
            IsRunningAll = false;
        }
    }

    private void ClearForm()
    {
        NewJobName = string.Empty;
        NewJobSource = string.Empty;
        NewJobTarget = string.Empty;
        _newJobType = SaveType.Full;
        OnPropertyChanged(nameof(IsFullType));
        OnPropertyChanged(nameof(IsDifferentialType));
        FormError = string.Empty;
    }

    private List<SaveJobViewModel> GetJobsFromCommand(string command)
    {
        var result = new List<SaveJobViewModel>();

        if (command.Equals("all", StringComparison.OrdinalIgnoreCase))
            return Jobs.ToList();

        if (int.TryParse(command, out int single))
        {
            AddJobByIndex(result, single);
            return result;
        }

        if (command.Contains('-'))
        {
            var parts = command.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int s) && int.TryParse(parts[1], out int e) && s <= e)
                for (int i = s; i <= e; i++) AddJobByIndex(result, i);
            return result;
        }

        if (command.Contains(';'))
        {
            var parts = command.Split(';');
            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int idx)) return new List<SaveJobViewModel>();
                AddJobByIndex(result, idx);
            }
            return result;
        }

        return result;
    }

    private void AddJobByIndex(List<SaveJobViewModel> list, int index)
    {
        int real = index - 1;
        if (real >= 0 && real < Jobs.Count && !list.Contains(Jobs[real]))
            list.Add(Jobs[real]);
    }

    #endregion
}
