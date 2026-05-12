using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace EasySave.Tests;

[TestClass]
public class ConfigServiceTests
{
    private string _testDirectory = string.Empty;
    private FakePathService _paths = null!;
    private ConfigService _configService = null!;

    [TestInitialize]
    public void Setup()
    {
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            "ConfigServiceTests",
            Guid.NewGuid().ToString()
        );

        Directory.CreateDirectory(_testDirectory);

        _paths = new FakePathService(_testDirectory);
        _configService = new ConfigService(_paths);
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
    public void LoadJobs_ShouldReturnEmptyList_WhenFileDoesNotExist()
    {
        // Act
        List<SaveJob> jobs = _configService.LoadJobs();

        // Assert
        Assert.AreEqual(0, jobs.Count);
    }

    [TestMethod]
    public void SaveJobs_ShouldCreateJobsFile()
    {
        // Arrange
        List<SaveJob> jobs = new List<SaveJob>
        {
            new SaveJob
            {
                Name = "Job1"
            }
        };

        // Act
        _configService.SaveJobs(jobs);

        // Assert
        Assert.IsTrue(File.Exists(_paths.JobsFile));
    }

    [TestMethod]
    public void LoadJobs_ShouldLoadSavedJobs()
    {
        // Arrange
        List<SaveJob> jobs = new List<SaveJob>
        {
            new SaveJob
            {
                Name = "BackupDocuments"
            }
        };

        _configService.SaveJobs(jobs);

        // Act
        List<SaveJob> loadedJobs = _configService.LoadJobs();

        // Assert
        Assert.AreEqual(1, loadedJobs.Count);
        Assert.AreEqual("BackupDocuments", loadedJobs[0].Name);
    }

    [TestMethod]
    public void LoadJobs_ShouldReturnEmptyList_WhenJsonIsInvalid()
    {
        // Arrange
        File.WriteAllText(_paths.JobsFile, "INVALID JSON");

        // Act
        List<SaveJob> jobs = _configService.LoadJobs();

        // Assert
        Assert.AreEqual(0, jobs.Count);
    }

    [TestMethod]
    public void GetLogFormat_ShouldReturnJson_WhenSettingsFileDoesNotExist()
    {
        // Act
        LogFormat format = _configService.GetLogFormat();

        // Assert
        Assert.AreEqual(LogFormat.JSON, format);
    }

    [TestMethod]
    public void SetLogFormat_ShouldSaveFormatIntoSettingsFile()
    {
        // Act
        _configService.SetLogFormat(LogFormat.XML);

        // Assert
        LogFormat format = _configService.GetLogFormat();

        Assert.AreEqual(LogFormat.XML, format);
    }

    [TestMethod]
    public void GetLogFormat_ShouldLoadSavedFormat()
    {
        // Arrange
        _configService.SetLogFormat(LogFormat.XML);

        // Act
        LogFormat format = _configService.GetLogFormat();

        // Assert
        Assert.AreEqual(LogFormat.XML, format);
    }

    [TestMethod]
    public void GetLogFormat_ShouldReturnJson_WhenSettingsJsonIsInvalid()
    {
        // Arrange
        File.WriteAllText(_paths.SettingsFile, "INVALID JSON");

        // Act
        LogFormat format = _configService.GetLogFormat();

        // Assert
        Assert.AreEqual(LogFormat.JSON, format);
    }

    private class FakePathService : IPathService
    {
        private readonly string _baseDirectory;

        public FakePathService(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public string JobsFile =>
            Path.Combine(_baseDirectory, "jobs.json");

        public string StateFile =>
            Path.Combine(_baseDirectory, "state.json");

        public string LogsDirectory =>
            Path.Combine(_baseDirectory, "Logs");

        public string SettingsFile =>
            Path.Combine(_baseDirectory, "settings.json");

        public void EnsureDirectoriesExist()
        {
        }
    }
}