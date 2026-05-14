using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using EasySave.Core.Model.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class FullSaveStrategyTests
{
    private string _rootDirectory = string.Empty;
    private string _sourceDirectory = string.Empty;
    private string _targetDirectory = string.Empty;

    private FakeLogger _logger = null!;
    private FakeStateService _stateService = null!;
    private FullSaveStrategy _strategy = null!;

    [TestInitialize]
    public void Setup()
    {
        _rootDirectory = Path.Combine(
            Path.GetTempPath(),
            "FullSaveStrategyTests",
            Guid.NewGuid().ToString()
        );

        _sourceDirectory = Path.Combine(_rootDirectory, "Source");
        _targetDirectory = Path.Combine(_rootDirectory, "Target");

        Directory.CreateDirectory(_sourceDirectory);
        Directory.CreateDirectory(_targetDirectory);

        _logger = new FakeLogger();
        _stateService = new FakeStateService();

        var appPaths = new AppPaths();
        var settingsService = new SettingsService(appPaths);
        var cryptoService = new CryptoService();

        _strategy = new FullSaveStrategy(
            _logger,
            _stateService,
            cryptoService,
            settingsService
        );
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_rootDirectory))
        {
            Directory.Delete(_rootDirectory, true);
        }
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldCopyFile_WhenSourceContainsOneFile()
    {
        string sourceFile = Path.Combine(_sourceDirectory, "file.txt");
        File.WriteAllText(sourceFile, "Hello");

        SaveJob job = CreateJob();

        _strategy.ExecuteSaveJob(job);

        string targetFile = Path.Combine(_targetDirectory, "file.txt");

        Assert.IsTrue(File.Exists(targetFile));
        Assert.AreEqual("Hello", File.ReadAllText(targetFile));
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldCopyAllFiles()
    {
        File.WriteAllText(Path.Combine(_sourceDirectory, "file1.txt"), "File 1");
        File.WriteAllText(Path.Combine(_sourceDirectory, "file2.txt"), "File 2");

        SaveJob job = CreateJob();

        _strategy.ExecuteSaveJob(job);

        Assert.IsTrue(File.Exists(Path.Combine(_targetDirectory, "file1.txt")));
        Assert.IsTrue(File.Exists(Path.Combine(_targetDirectory, "file2.txt")));
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldOverwriteExistingTargetFile()
    {
        string sourceFile = Path.Combine(_sourceDirectory, "file.txt");
        string targetFile = Path.Combine(_targetDirectory, "file.txt");

        File.WriteAllText(sourceFile, "New content");
        File.WriteAllText(targetFile, "Old content");

        SaveJob job = CreateJob();

        _strategy.ExecuteSaveJob(job);

        Assert.AreEqual("New content", File.ReadAllText(targetFile));
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldPreserveSubDirectoryStructure()
    {
        string subDirectory = Path.Combine(_sourceDirectory, "FolderA");
        Directory.CreateDirectory(subDirectory);

        string sourceFile = Path.Combine(subDirectory, "file.txt");
        File.WriteAllText(sourceFile, "Hello subfolder");

        SaveJob job = CreateJob();

        _strategy.ExecuteSaveJob(job);

        string targetFile = Path.Combine(_targetDirectory, "FolderA", "file.txt");

        Assert.IsTrue(File.Exists(targetFile));
        Assert.AreEqual("Hello subfolder", File.ReadAllText(targetFile));
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldCopyTwoFiles()
    {
        File.WriteAllText(Path.Combine(_sourceDirectory, "file1.txt"), "File 1");
        File.WriteAllText(Path.Combine(_sourceDirectory, "file2.txt"), "File 2");

        SaveJob job = CreateJob();

        _strategy.ExecuteSaveJob(job);

        string[] copiedFiles = Directory.GetFiles(_targetDirectory);

        Assert.AreEqual(2, copiedFiles.Length);
        Assert.IsTrue(File.Exists(Path.Combine(_targetDirectory, "file1.txt")));
        Assert.IsTrue(File.Exists(Path.Combine(_targetDirectory, "file2.txt")));
    }

    [TestMethod]
    public void ExecuteSaveJob_ShouldNotThrow_WhenCopyingMultipleFiles()
    {
        File.WriteAllText(Path.Combine(_sourceDirectory, "file1.txt"), "File 1");
        File.WriteAllText(Path.Combine(_sourceDirectory, "file2.txt"), "File 2");

        SaveJob job = CreateJob();

        _strategy.ExecuteSaveJob(job);

        Assert.IsTrue(File.Exists(Path.Combine(_targetDirectory, "file1.txt")));
        Assert.IsTrue(File.Exists(Path.Combine(_targetDirectory, "file2.txt")));
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
        public List<LogEntry> Entries { get; } = new();

        public void Log(LogEntry entry)
        {
            Entries.Add(entry);
        }
    }

    private class FakeStateService : IStateService
    {
        public List<SaveState> States { get; } = new();

        public void UpdateState(SaveState state)
        {
            States.Add(state);
        }
    }
}