using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using EasySave.Core.Model.Strategies;
using EasySave.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class SaveJobViewModelTests
{
    private string _testDirectory = string.Empty;
    private string _sourceDirectory = string.Empty;
    private string _targetDirectory = string.Empty;
    private SaveExecutor _saveExecutor = null!;
    private LanguageService _languageService = null!;

    [TestInitialize]
    public void Setup()
    {
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            "SaveJobViewModelTests",
            Guid.NewGuid().ToString()
        );

        _sourceDirectory = Path.Combine(_testDirectory, "Source");
        _targetDirectory = Path.Combine(_testDirectory, "Target");

        Directory.CreateDirectory(_sourceDirectory);

        _languageService = new LanguageService();

        var settings = new GeneralSettings
        {
            PriorityExtensions = new List<string> { ".txt" },
            LargeFileLimitKo = 10000
        };

        var priorityFileService = new PriorityFileService(settings);
        var largeFileTransferService = new LargeFileTransferService(settings);

        _saveExecutor = new SaveExecutor(
            new FakeSaveStrategy(),
            new FakeSaveStrategy(),
            new FakeLogger(),
            new FakeStateService(),
            priorityFileService,
            largeFileTransferService
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
    public void IsValid_ShouldReturnFalse_WhenNameIsEmpty()
    {
        SaveJobViewModel vm = CreateViewModel();
        vm.Name = "";
        vm.SourceFolder = _sourceDirectory;
        vm.TargetFolder = _targetDirectory;

        bool result = vm.IsValid();

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_ShouldReturnFalse_WhenSourceFolderDoesNotExist()
    {
        SaveJobViewModel vm = CreateViewModel();
        vm.Name = "Job1";
        vm.SourceFolder = Path.Combine(_testDirectory, "UnknownSource");
        vm.TargetFolder = _targetDirectory;

        bool result = vm.IsValid();

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_ShouldReturnTrue_WhenRequiredFieldsAreValid()
    {
        SaveJobViewModel vm = CreateViewModel();
        vm.Name = "Job1";
        vm.SourceFolder = _sourceDirectory;
        vm.TargetFolder = _targetDirectory;

        bool result = vm.IsValid();

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CreateJob_ShouldCreateSaveJobFromViewModelProperties()
    {
        SaveJobViewModel vm = CreateViewModel();
        vm.Name = "Job1";
        vm.SourceFolder = _sourceDirectory;
        vm.TargetFolder = _targetDirectory;
        vm.Type = SaveType.Differential;

        SaveJob job = vm.CreateJob();

        Assert.AreEqual("Job1", job.Name);
        Assert.AreEqual(_sourceDirectory, job.SourceFolder);
        Assert.AreEqual(_targetDirectory, job.TargetFolder);
        Assert.AreEqual(SaveType.Differential, job.Type);
    }

    [TestMethod]
    public async Task Execute_ShouldSetInvalidMessage_WhenJobIsInvalid()
    {
        SaveJobViewModel vm = CreateViewModel();
        vm.Name = "";
        vm.SourceFolder = _sourceDirectory;
        vm.TargetFolder = _targetDirectory;

        await vm.Execute();

        Assert.AreEqual(_languageService.GetText("job.invalid"), vm.ResultMessage);
    }

    [TestMethod]
    public async Task Execute_ShouldCreateTargetDirectory_WhenItDoesNotExist()
    {
        SaveJobViewModel vm = CreateValidViewModel();

        await vm.Execute();

        Assert.IsTrue(Directory.Exists(_targetDirectory));
    }

    [TestMethod]
    public async Task Execute_ShouldSetStatusDoneAndProgress100_WhenExecutionSucceeds()
    {
        SaveJobViewModel vm = CreateValidViewModel();

        await vm.Execute();

        Assert.AreEqual(_languageService.GetText("status.done"), vm.Status);
        Assert.AreEqual(_languageService.GetText("job.success"), vm.ResultMessage);
        Assert.AreEqual(100, vm.ProgressValue);
        Assert.IsFalse(vm.IsRunning);
    }

    private SaveJobViewModel CreateViewModel()
    {
        return new SaveJobViewModel(_saveExecutor, _languageService);
    }

    private SaveJobViewModel CreateValidViewModel()
    {
        return new SaveJobViewModel(_saveExecutor, _languageService)
        {
            Name = "Job1",
            SourceFolder = _sourceDirectory,
            TargetFolder = _targetDirectory,
            Type = SaveType.Full
        };
    }

    private class FakeSaveStrategy : ISaveStrategy
    {
        public void ExecuteSaveJob(
            SaveJob job,
            CancellationToken cancellationToken = default,
            IProgress<SaveState>? progress = null,
            PriorityFileService? priorityFileService = null,
            LargeFileTransferService? largeFileTransferService = null)
        {
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