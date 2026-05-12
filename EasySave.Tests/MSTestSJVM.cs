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

        _saveExecutor = new SaveExecutor(
            new FakeSaveStrategy(),
            new FakeSaveStrategy(),
            new FakeLogger(),
            new FakeStateService()
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
        // Arrange
        SaveJobViewModel vm = CreateViewModel();
        vm.Name = "";
        vm.SourceFolder = _sourceDirectory;
        vm.TargetFolder = _targetDirectory;

        // Act
        bool result = vm.IsValid();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_ShouldReturnFalse_WhenSourceFolderDoesNotExist()
    {
        // Arrange
        SaveJobViewModel vm = CreateViewModel();
        vm.Name = "Job1";
        vm.SourceFolder = Path.Combine(_testDirectory, "UnknownSource");
        vm.TargetFolder = _targetDirectory;

        // Act
        bool result = vm.IsValid();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_ShouldReturnTrue_WhenRequiredFieldsAreValid()
    {
        // Arrange
        SaveJobViewModel vm = CreateViewModel();
        vm.Name = "Job1";
        vm.SourceFolder = _sourceDirectory;
        vm.TargetFolder = _targetDirectory;

        // Act
        bool result = vm.IsValid();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CreateJob_ShouldCreateSaveJobFromViewModelProperties()
    {
        // Arrange
        SaveJobViewModel vm = CreateViewModel();
        vm.Name = "Job1";
        vm.SourceFolder = _sourceDirectory;
        vm.TargetFolder = _targetDirectory;
        vm.Type = SaveType.Differential;

        // Act
        SaveJob job = vm.CreateJob();

        // Assert
        Assert.AreEqual("Job1", job.Name);
        Assert.AreEqual(_sourceDirectory, job.SourceFolder);
        Assert.AreEqual(_targetDirectory, job.TargetFolder);
        Assert.AreEqual(SaveType.Differential, job.Type);
    }

    [TestMethod]
    public async Task Execute_ShouldSetInvalidMessage_WhenJobIsInvalid()
    {
        // Arrange
        SaveJobViewModel vm = CreateViewModel();
        vm.Name = "";
        vm.SourceFolder = _sourceDirectory;
        vm.TargetFolder = _targetDirectory;

        // Act
        await vm.Execute();

        // Assert
        Assert.AreEqual(_languageService.GetText("job.invalid"), vm.ResultMessage);
    }

    [TestMethod]
    public async Task Execute_ShouldCreateTargetDirectory_WhenItDoesNotExist()
    {
        // Arrange
        SaveJobViewModel vm = CreateValidViewModel();

        // Act
        await vm.Execute();

        // Assert
        Assert.IsTrue(Directory.Exists(_targetDirectory));
    }

    [TestMethod]
    public async Task Execute_ShouldSetStatusDoneAndProgress100_WhenExecutionSucceeds()
    {
        // Arrange
        SaveJobViewModel vm = CreateValidViewModel();

        // Act
        await vm.Execute();

        // Assert
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
        public void ExecuteSaveJob(SaveJob job)
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