using System.Diagnostics;
using System.Globalization;
using System.Windows;
using EasySave.Core.Model.Service;
using EasySave.Core.Model.Strategies;
using EasySave.ViewModel;

namespace EasySave.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // === Dependency wiring (manual DI) ===
            var paths = new AppPaths();
            paths.EnsureDirectoriesExist();

            var languageService   = new LanguageService();
            var configService     = new ConfigService(paths);
            var settingsService   = new SettingsService(paths);
            var stateService      = new StateService(paths);
            var businessService   = new BusinessSoftwareService();

            // Load settings to configure culture and log format
            var settings = settingsService.LoadSettings();

            // Set culture for localized .resx resources
            var culture = new CultureInfo(settings.Language ?? "en");
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture   = culture;

            languageService.SetLanguage(settings.Language ?? "en");

            var logger = EasyLog.LoggerFactory.CreateLogger(
                settings.LogFormat, paths.LogsDirectory);

            var fullStrategy  = new FullSaveStrategy(logger, stateService, new CryptoService(), settingsService);
            var diffStrategy  = new DifferentialSaveStrategy(logger, stateService, new CryptoService(), settingsService);
            var saveExecutor  = new SaveExecutor(fullStrategy, diffStrategy, logger, stateService);

            var jobListVm = new SaveJobListViewModel(
                configService, languageService, saveExecutor,
                businessService, settingsService);
            jobListVm.LoadJobs();

            var settingsVm  = new SettingsViewModel(settingsService, languageService);
            settingsVm.RequestRestart = RestartApp;
            var mainVm      = new MainViewModel(jobListVm, settingsVm);

            var window = new MainWindow { DataContext = mainVm };
            window.Show();
        }

        private static void RestartApp()
        {
            var exe = Process.GetCurrentProcess().MainModule?.FileName;
            if (!string.IsNullOrEmpty(exe))
                Process.Start(new ProcessStartInfo { FileName = exe, UseShellExecute = true });
            Current.Shutdown();
        }
    }
}

