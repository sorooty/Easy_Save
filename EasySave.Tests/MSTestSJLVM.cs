using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using EasySave.Core.Model.Strategies;
using EasySave.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class SaveJobListViewModelTests
{
    private string _testDirectory = string.Empty;
    private FakePathService _paths = null!;
    private ConfigService _configService = null!;
    private LanguageService _languageService = null!;
    private SaveExecutor _saveExecutor = null!;
    private SaveJobListViewModel _viewModel = null!;

    [TestInitialize]
    public void Setup()
    {
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            "SaveJobListViewModelTests",
            Guid.NewGuid().ToString()
        );

        Directory.CreateDirectory(_testDirectory);

        _paths = new FakePathService(_testDirectory);
        _configService = new ConfigService(_paths);
        _languageService = new LanguageService();

        _saveExecutor = new SaveExecutor(
            new FakeSaveStrategy(),
            new FakeSaveStrategy(),
            new FakeLogger(),
            new FakeStateService()
        );

        _viewModel = new SaveJobListViewModel(
            _configService,
            _languageService,
            _saveExecutor
        );
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    [TestMethod]
    public void AddJob_ShouldAddJob_WhenJobIsValid()
    {
        // Arrange
        string source = CreateDirectory("Source");
        string target = CreateDirectory("Target");

        // Act
        bool result = _viewModel.AddJob("Job1", source, target, "1");

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(1, _viewModel.Jobs.Count);
        Assert.AreEqual("Job1", _viewModel.Jobs[0].Name);
        Assert.AreEqual(SaveType.Full, _viewModel.Jobs[0].Type);
    }

    [TestMethod]
    public void AddJob_ShouldReturnFalse_WhenJobNameAlreadyExists()
    {
        // Arrange
        string source = CreateDirectory("Source");
        string target = CreateDirectory("Target");

        _viewModel.AddJob("Job1", source, target, "1");

        // Act
        bool result = _viewModel.AddJob("Job1", source, target, "2");

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(1, _viewModel.Jobs.Count);
    }

    [TestMethod]
    public void RemoveJobByName_ShouldRemoveJob_WhenJobExists()
    {
        // Arrange
        string source = CreateDirectory("Source");
        string target = CreateDirectory("Target");

        _viewModel.AddJob("Job1", source, target, "1");

        // Act
        bool result = _viewModel.RemoveJobByName("Job1");

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(0, _viewModel.Jobs.Count);
    }

    [TestMethod]
    public void RemoveJobByName_ShouldReturnFalse_WhenJobDoesNotExist()
    {
        // Act
        bool result = _viewModel.RemoveJobByName("UnknownJob");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void SaveJobs_ShouldPersistJobs()
    {
        // Arrange
        string source = CreateDirectory("Source");
        string target = CreateDirectory("Target");

        _viewModel.AddJob("Job1", source, target, "1");

        // Act
        List<SaveJob> savedJobs = _configService.LoadJobs();

        // Assert
        Assert.AreEqual(1, savedJobs.Count);
        Assert.AreEqual("Job1", savedJobs[0].Name);
    }

    [TestMethod]
    public void LoadJobs_ShouldLoadSavedJobs()
    {
        // Arrange
        string source = CreateDirectory("Source");
        string target = CreateDirectory("Target");

        _configService.SaveJobs(new List<SaveJob>
        {
            new SaveJob
            {
                Name = "Job1",
                SourceFolder = source,
                TargetFolder = target,
                Type = SaveType.Differential
            }
        });

        // Act
        _viewModel.LoadJobs();

        // Assert
        Assert.AreEqual(1, _viewModel.Jobs.Count);
        Assert.AreEqual("Job1", _viewModel.Jobs[0].Name);
        Assert.AreEqual(SaveType.Differential, _viewModel.Jobs[0].Type);
    }

    [TestMethod]
    public void SetLogFormat_ShouldChangeLogFormat()
    {
        // Act
        _viewModel.SetLogFormat(LogFormat.XML);

        // Assert
        Assert.AreEqual(LogFormat.XML, _viewModel.GetLogFormat());
    }

    private string CreateDirectory(string name)
    {
        string path = Path.Combine(_testDirectory, name);
        Directory.CreateDirectory(path);
        return path;
    }

    private class FakePathService : IPathService
    {
        private readonly string _baseDirectory;

        public FakePathService(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public string JobsFile => Path.Combine(_baseDirectory, "jobs.json");
        public string StateFile => Path.Combine(_baseDirectory, "state.json");
        public string LogsDirectory => Path.Combine(_baseDirectory, "Logs");
        public string SettingsFile => Path.Combine(_baseDirectory, "settings.json");

        public void EnsureDirectoriesExist()
        {
        }
    }

    private class FakeSaveStrategy : ISaveStrategy
    {
        public int ExecutionCount { get; private set; }

        public void ExecuteSaveJob(SaveJob job)
        {
            ExecutionCount++;
        }
    }

    private class FakeLogger : ILogger
    {
        public void Log(LogEntry entry)
        {
        }
    }

    private class FakeStateService : IStateService
    {
        public void UpdateState(SaveState state)
        {
        }
    }
}