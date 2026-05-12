using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace EasySave.Tests;

[TestClass]
public class StateServiceTests
{
    private string _testDirectory = string.Empty;
    private FakePathService _paths = null!;
    private StateService _stateService = null!;

    [TestInitialize]
    public void Setup()
    {
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            "StateServiceTests",
            Guid.NewGuid().ToString()
        );

        Directory.CreateDirectory(_testDirectory);

        _paths = new FakePathService(_testDirectory);
        _stateService = new StateService(_paths);
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
    public void UpdateState_ShouldCreateStateFile()
    {
        // Arrange
        SaveState state = new SaveState
        {
            Name = "Job1",
            Status = "Active"
        };

        // Act
        _stateService.UpdateState(state);

        // Assert
        Assert.IsTrue(File.Exists(_paths.StateFile));
    }

    [TestMethod]
    public void UpdateState_ShouldAddNewState_WhenJobDoesNotExist()
    {
        // Arrange
        SaveState state = new SaveState
        {
            Name = "Job1",
            Status = "Active"
        };

        // Act
        _stateService.UpdateState(state);

        // Assert
        List<SaveState> states = ReadStates();

        Assert.AreEqual(1, states.Count);
        Assert.AreEqual("Job1", states[0].Name);
        Assert.AreEqual("Active", states[0].Status);
    }

    [TestMethod]
    public void UpdateState_ShouldReplaceExistingState_WhenJobAlreadyExists()
    {
        // Arrange
        SaveState firstState = new SaveState
        {
            Name = "Job1",
            Status = "Active",
            ProgressPercent = 20
        };

        SaveState updatedState = new SaveState
        {
            Name = "Job1",
            Status = "Completed",
            ProgressPercent = 100
        };

        // Act
        _stateService.UpdateState(firstState);
        _stateService.UpdateState(updatedState);

        // Assert
        List<SaveState> states = ReadStates();

        Assert.AreEqual(1, states.Count);
        Assert.AreEqual("Job1", states[0].Name);
        Assert.AreEqual("Completed", states[0].Status);
        Assert.AreEqual(100, states[0].ProgressPercent);
    }

    [TestMethod]
    public void UpdateState_ShouldKeepOtherStates_WhenUpdatingOneJob()
    {
        // Arrange
        _stateService.UpdateState(new SaveState
        {
            Name = "Job1",
            Status = "Active"
        });

        _stateService.UpdateState(new SaveState
        {
            Name = "Job2",
            Status = "Active"
        });

        // Act
        _stateService.UpdateState(new SaveState
        {
            Name = "Job1",
            Status = "Completed"
        });

        // Assert
        List<SaveState> states = ReadStates();

        Assert.AreEqual(2, states.Count);

        SaveState job1 = states.First(s => s.Name == "Job1");
        SaveState job2 = states.First(s => s.Name == "Job2");

        Assert.AreEqual("Completed", job1.Status);
        Assert.AreEqual("Active", job2.Status);
    }

    [TestMethod]
    public void UpdateState_ShouldReplaceCorruptedFileContent()
    {
        // Arrange
        File.WriteAllText(_paths.StateFile, "INVALID JSON");

        SaveState state = new SaveState
        {
            Name = "Job1",
            Status = "Active"
        };

        // Act
        _stateService.UpdateState(state);

        // Assert
        List<SaveState> states = ReadStates();

        Assert.AreEqual(1, states.Count);
        Assert.AreEqual("Job1", states[0].Name);
        Assert.AreEqual("Active", states[0].Status);
    }

    private List<SaveState> ReadStates()
    {
        string json = File.ReadAllText(_paths.StateFile);

        return JsonSerializer.Deserialize<List<SaveState>>(json)
            ?? new List<SaveState>();
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