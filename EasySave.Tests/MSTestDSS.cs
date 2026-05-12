using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using EasySave.Core.Model.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class DifferentialSaveStrategyTests
{
    private string _sourceDirectory = string.Empty;
    private string _targetDirectory = string.Empty;
    private FakeLogger _logger = null!;
    private FakeStateService _stateService = null!;
    private DifferentialSaveStrategy _strategy = null!;

    [TestInitialize]
    public void Setup()
    {
        string root = Path.Combine(
            Path.GetTempPath(),
            "DifferentialSaveStrategyTests",
            Guid.NewGuid().ToString()
        );

        _sourceDirectory = Path.Combine(root, "Source");
        _targetDirectory = Path.Combine(root, "Target");

        Directory.CreateDirectory(_sourceDirectory);
        Directory.CreateDirectory(_targetDirectory);

        _logger = new FakeLogger();
        _stateService = new FakeStateService();
        _strategy = new DifferentialSaveStrategy(_logger, _stateService);
    }

    [TestCleanup]
    public void Cleanup()
    {
        string root = Directory.GetParent(_sourceDirectory)!.FullName;

        if (Directory.Exists(root))
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldCopyFile_WhenTargetFileDoesNotExist()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceDirectory, "file.txt");
        File.WriteAllText(sourceFile, "Hello");

        SaveJob job = CreateJob();

        // Act
        _strategy.ExecuteSaveJob(job);

        // Assert
        string targetFile = Path.Combine(_targetDirectory, "file.txt");

        Assert.IsTrue(File.Exists(targetFile));
        Assert.AreEqual("Hello", File.ReadAllText(targetFile));
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldNotCopyFile_WhenTargetFileIsUpToDate()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceDirectory, "file.txt");
        string targetFile = Path.Combine(_targetDirectory, "file.txt");

        File.WriteAllText(sourceFile, "Source");
        File.WriteAllText(targetFile, "Target");

        DateTime sameDate = DateTime.UtcNow;

        File.SetLastWriteTimeUtc(sourceFile, sameDate);
        File.SetLastWriteTimeUtc(targetFile, sameDate);

        SaveJob job = CreateJob();

        // Act
        _strategy.ExecuteSaveJob(job);

        // Assert
        Assert.AreEqual("Target", File.ReadAllText(targetFile));
        Assert.AreEqual(0, _logger.Entries.Count);
        Assert.AreEqual(0, _stateService.States.Count);
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldCopyFile_WhenSourceIsMoreRecentThanTarget()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceDirectory, "file.txt");
        string targetFile = Path.Combine(_targetDirectory, "file.txt");

        File.WriteAllText(sourceFile, "New content");
        File.WriteAllText(targetFile, "Old content");

        File.SetLastWriteTimeUtc(targetFile, DateTime.UtcNow.AddMinutes(-10));
        File.SetLastWriteTimeUtc(sourceFile, DateTime.UtcNow);

        SaveJob job = CreateJob();

        // Act
        _strategy.ExecuteSaveJob(job);

        // Assert
        Assert.AreEqual("New content", File.ReadAllText(targetFile));
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldPreserveSubDirectoryStructure()
    {
        // Arrange
        string subDirectory = Path.Combine(_sourceDirectory, "FolderA");
        Directory.CreateDirectory(subDirectory);

        string sourceFile = Path.Combine(subDirectory, "file.txt");
        File.WriteAllText(sourceFile, "Hello subfolder");

        SaveJob job = CreateJob();

        // Act
        _strategy.ExecuteSaveJob(job);

        // Assert
        string targetFile = Path.Combine(_targetDirectory, "FolderA", "file.txt");

        Assert.IsTrue(File.Exists(targetFile));
        Assert.AreEqual("Hello subfolder", File.ReadAllText(targetFile));
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldWriteLog_WhenFileIsCopied()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceDirectory, "file.txt");
        File.WriteAllText(sourceFile, "Hello");

        SaveJob job = CreateJob();

        // Act
        _strategy.ExecuteSaveJob(job);

        // Assert
        Assert.AreEqual(1, _logger.Entries.Count);
        Assert.AreEqual("Job1", _logger.Entries[0].JobName);
        Assert.AreEqual(sourceFile, _logger.Entries[0].SourceFile);
        Assert.AreEqual("OK", _logger.Entries[0].State);
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldUpdateState_WhenFileIsCopied()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceDirectory, "file.txt");
        File.WriteAllText(sourceFile, "Hello");

        SaveJob job = CreateJob();

        // Act
        _strategy.ExecuteSaveJob(job);

        // Assert
        Assert.AreEqual(1, _stateService.States.Count);
        Assert.AreEqual("Job1", _stateService.States[0].Name);
        Assert.AreEqual("Active", _stateService.States[0].Status);
        Assert.AreEqual(sourceFile, _stateService.States[0].CurrentSourceFile);
    }

    private SaveJob CreateJob()
    {
        return new SaveJob
        {
            Name = "Job1",
            SourceFolder = _sourceDirectory,
            TargetFolder = _targetDirectory,
            Type = SaveType.Differential
        };
    }

    private class FakeLogger : ILogger
    {
        public List<LogEntry> Entries { get; } = new List<LogEntry>();

        public void Log(LogEntry entry)
        {
            Entries.Add(entry);
        }
    }

    private class FakeStateService : IStateService
    {
        public List<SaveState> States { get; } = new List<SaveState>();

        public void UpdateState(SaveState state)
        {
            States.Add(state);
        }
    }
}