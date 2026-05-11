using EasySave.View;
using EasySave.ViewModel;
using EasySave.Core.Model.Service;
using EasySave.Core.Model.Strategies;
using EasyLog;

using System;

class Program
{
    static async Task Main(string[] args)
    {
        // Paths
        IPathService pathService = new AppPaths();
        pathService.EnsureDirectoriesExist();

        // Services
        var languageService = new LanguageService();
        var configService = new ConfigService(pathService);

        LogFormat logFormat = configService.GetLogFormat();
        var logger = LoggerFactory.CreateLogger(logFormat, pathService.LogsDirectory);

        var stateService = new StateService(pathService);
        var fullStrategy = new FullSaveStrategy(logger, stateService);
        var differentialStrategy = new DifferentialSaveStrategy(logger, stateService);

        var saveExecutor = new SaveExecutor(fullStrategy, differentialStrategy, logger, stateService);

        // ViewModel
        var viewModel = new SaveJobListViewModel(
            configService,
            languageService,
            saveExecutor
        );

        viewModel.LoadJobs();

        // View
        var view = new ConsoleView(viewModel);

        await view.Run();
    }
}