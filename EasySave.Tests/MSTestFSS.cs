using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using EasySave.Core.Model.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class FullSaveStrategyTests
{
    private string _sourceDirectory = string.Empty;
    private string _targetDirectory = string.Empty;
    private FakeLogger _logger = null!;
    private FakeStateService _stateService = null!;
    private FullSaveStrategy _strategy = null!;

    [TestInitialize]
    public void Setup()
    {
        string root = Path.Combine(
            Path.GetTempPath(),
            "FullSaveStrategyTests",
            Guid.NewGuid().ToString()
        );

        _sourceDirectory = Path.Combine(root, "Source");
        _targetDirectory = Path.Combine(root, "Target");

        Directory.CreateDirectory(_sourceDirectory);
        Directory.CreateDirectory(_targetDirectory);

        _logger = new FakeLogger();
        _stateService = new FakeStateService();
        _strategy = new FullSaveStrategy(_logger, _stateService, new CryptoService(), new SettingsService(new AppPaths()));
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
    public void ExecuteSaveJob_ShouldCopyFile_WhenSourceContainsOneFile()
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
    public void ExecuteSaveJob_ShouldCopyAllFiles()
    {
        // Arrange
        File.WriteAllText(Path.Combine(_sourceDirectory, "file1.txt"), "File 1");
        File.WriteAllText(Path.Combine(_sourceDirectory, "file2.txt"), "File 2");

        SaveJob job = CreateJob();

        // Act
        _strategy.ExecuteSaveJob(job);

        // Assert
        Assert.IsTrue(File.Exists(Path.Combine(_targetDirectory, "file1.txt")));
        Assert.IsTrue(File.Exists(Path.Combine(_targetDirectory, "file2.txt")));
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldOverwriteExistingTargetFile()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceDirectory, "file.txt");
        string targetFile = Path.Combine(_targetDirectory, "file.txt");

        File.WriteAllText(sourceFile, "New content");
        File.WriteAllText(targetFile, "Old content");

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
    public void ExecuteSaveJob_ShouldWriteOneLogPerCopiedFile()
    {
        // Arrange
        File.WriteAllText(Path.Combine(_sourceDirectory, "file1.txt"), "File 1");
        File.WriteAllText(Path.Combine(_sourceDirectory, "file2.txt"), "File 2");

        SaveJob job = CreateJob();

        // Act
        _strategy.ExecuteSaveJob(job);

        // Assert
        Assert.AreEqual(2, _logger.Entries.Count);
        Assert.IsTrue(_logger.Entries.All(e => e.JobName == "Job1"));
        Assert.IsTrue(_logger.Entries.All(e => e.State == "OK"));
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldUpdateStateForEachCopiedFile()
    {
        // Arrange
        File.WriteAllText(Path.Combine(_sourceDirectory, "file1.txt"), "File 1");
        File.WriteAllText(Path.Combine(_sourceDirectory, "file2.txt"), "File 2");

        SaveJob job = CreateJob();

        // Act
        _strategy.ExecuteSaveJob(job);

        // Assert
        Assert.AreEqual(2, _stateService.States.Count);
        Assert.IsTrue(_stateService.States.All(s => s.Name == "Job1"));
        Assert.IsTrue(_stateService.States.All(s => s.Status == "Active"));
    }

    private SaveJob CreateJob()
    {
        return new SaveJob
        {
            Name = "Job1",
            SourceFolder = _sourceDirectory,
            TargetFolder = _targetDirectory,
            Type = SaveType.Full
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