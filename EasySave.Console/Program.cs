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

        // Services
        var languageService = new LanguageService();
        var configService = new ConfigService(pathService);

        var logger = new JsonLogger(pathService.LogsDirectory);
        var stateService = new StateService(pathService);
        var fullStrategy = new FullSaveStrategy(logger, stateService);
        var differentialStrategy = new DifferentialSaveStrategy(logger, stateService);


        var saveExecutor = new SaveExecutor(fullStrategy, logger, stateService);

        // ViewModel
        var viewModel = new SaveJobListViewModel(
            configService,
            languageService,
            saveExecutor
        );

        viewModel.LoadJobs();
        Console.WriteLine(pathService.JobsFile);

        // View
        var view = new ConsoleView(viewModel);

        await view.Run();
    }
}