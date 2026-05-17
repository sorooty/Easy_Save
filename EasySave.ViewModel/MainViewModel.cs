namespace EasySave.ViewModel;

/// <summary>
/// ViewModel principal de l'application WPF.
/// Gere la navigation entre Jobs, Parametres et Aide.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private readonly SaveJobListViewModel _jobs;
    private readonly SettingsViewModel _settings;
    private readonly HelpViewModel _help;
    private ViewModelBase _currentView;
    private string _currentPage = "jobs";

    public ViewModelBase CurrentView
    {
        get => _currentView;
        private set => Set(ref _currentView, value);
    }

    public SaveJobListViewModel Jobs => _jobs;
    public SettingsViewModel Settings => _settings;

    public bool IsJobsPage
    {
        get => _currentPage == "jobs";
        private set
        {
            if (value) _currentPage = "jobs";
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsSettingsPage));
            OnPropertyChanged(nameof(IsHelpPage));
        }
    }

    public bool IsSettingsPage
    {
        get => _currentPage == "settings";
        private set
        {
            if (value) _currentPage = "settings";
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsJobsPage));
            OnPropertyChanged(nameof(IsHelpPage));
        }
    }

    public bool IsHelpPage
    {
        get => _currentPage == "help";
        private set
        {
            if (value) _currentPage = "help";
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsJobsPage));
            OnPropertyChanged(nameof(IsSettingsPage));
        }
    }

    public RelayCommand ShowJobsCommand { get; }
    public RelayCommand ShowSettingsCommand { get; }
    public RelayCommand ShowHelpCommand { get; }

    public MainViewModel(SaveJobListViewModel jobs, SettingsViewModel settings)
    {
        _jobs = jobs;
        _settings = settings;
        _help = new HelpViewModel();
        _currentView = jobs;

        ShowJobsCommand = new RelayCommand(_ =>
        {
            CurrentView = _jobs;
            IsJobsPage = true;
        });

        ShowSettingsCommand = new RelayCommand(_ =>
        {
            CurrentView = _settings;
            IsSettingsPage = true;
        });

        ShowHelpCommand = new RelayCommand(_ =>
        {
            CurrentView = _help;
            IsHelpPage = true;
        });
    }
}
