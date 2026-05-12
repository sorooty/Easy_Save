namespace EasySave.ViewModel;

/// <summary>
/// ViewModel principal de l'application WPF.
/// Gère la navigation entre la vue Jobs et la vue Paramètres.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private readonly SaveJobListViewModel _jobs;
    private readonly SettingsViewModel _settings;
    private ViewModelBase _currentView;
    private bool _isJobsPage = true;

    public ViewModelBase CurrentView
    {
        get => _currentView;
        private set => Set(ref _currentView, value);
    }

    public SaveJobListViewModel Jobs => _jobs;
    public SettingsViewModel Settings => _settings;

    public bool IsJobsPage
    {
        get => _isJobsPage;
        private set
        {
            Set(ref _isJobsPage, value);
            OnPropertyChanged(nameof(IsSettingsPage));
        }
    }

    public bool IsSettingsPage => !_isJobsPage;

    public RelayCommand ShowJobsCommand { get; }
    public RelayCommand ShowSettingsCommand { get; }

    public MainViewModel(SaveJobListViewModel jobs, SettingsViewModel settings)
    {
        _jobs = jobs;
        _settings = settings;
        _currentView = jobs;

        ShowJobsCommand = new RelayCommand(_ =>
        {
            CurrentView = _jobs;
            IsJobsPage = true;
        });

        ShowSettingsCommand = new RelayCommand(_ =>
        {
            CurrentView = _settings;
            IsJobsPage = false;
        });
    }
}
