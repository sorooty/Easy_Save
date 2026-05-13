using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using EasySave.Core.Model.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class SaveExecutorTests
{
    [TestMethod]
    public async Task ExecuteAsync_ShouldUseFullStrategy_WhenJobTypeIsFull()
    {
        // Arrange
        FakeSaveStrategy fullStrategy = new FakeSaveStrategy();
        FakeSaveStrategy differentialStrategy = new FakeSaveStrategy();
        FakeLogger logger = new FakeLogger();
        FakeStateService stateService = new FakeStateService();

        SaveExecutor executor = new SaveExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService
        );

        SaveJob job = new SaveJob
        {
            Name = "Job1",
            Type = SaveType.Full
        };

        // Act
        await executor.ExecuteAsync(job, null, CancellationToken.None);

        // Assert
        Assert.AreEqual(1, fullStrategy.ExecutionCount);
        Assert.AreEqual(0, differentialStrategy.ExecutionCount);
    }

    [TestMethod]
    public async Task ExecuteAsync_ShouldUseDifferentialStrategy_WhenJobTypeIsDifferential()
    {
        // Arrange
        FakeSaveStrategy fullStrategy = new FakeSaveStrategy();
        FakeSaveStrategy differentialStrategy = new FakeSaveStrategy();
        FakeLogger logger = new FakeLogger();
        FakeStateService stateService = new FakeStateService();

        SaveExecutor executor = new SaveExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService
        );

        SaveJob job = new SaveJob
        {
            Name = "Job1",
            Type = SaveType.Differential
        };

        // Act
        await executor.ExecuteAsync(job, null, CancellationToken.None);

        // Assert
        Assert.AreEqual(0, fullStrategy.ExecutionCount);
        Assert.AreEqual(1, differentialStrategy.ExecutionCount);
    }

    [TestMethod]
    public async Task ExecuteAsync_ShouldUpdateStateToActiveThenCompleted_WhenExecutionSucceeds()
    {
        // Arrange
        FakeSaveStrategy fullStrategy = new FakeSaveStrategy();
        FakeSaveStrategy differentialStrategy = new FakeSaveStrategy();
        FakeLogger logger = new FakeLogger();
        FakeStateService stateService = new FakeStateService();

        SaveExecutor executor = new SaveExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService
        );

        SaveJob job = new SaveJob
        {
            Name = "Job1",
            Type = SaveType.Full
        };

        // Act
        await executor.ExecuteAsync(job, null, CancellationToken.None);

        // Assert
        Assert.AreEqual(2, stateService.States.Count);
        Assert.AreEqual("Active", stateService.States[0].Status);
        Assert.AreEqual("Completed", stateService.States[1].Status);
        Assert.AreEqual(100, stateService.States[1].ProgressPercent);
    }

    [TestMethod]
    public async Task ExecuteAsync_ShouldUpdateStateToError_WhenStrategyThrowsException()
    {
        // Arrange
        FakeSaveStrategy fullStrategy = new FakeSaveStrategy
        {
            ShouldThrowException = true
        };

        FakeSaveStrategy differentialStrategy = new FakeSaveStrategy();
        FakeLogger logger = new FakeLogger();
        FakeStateService stateService = new FakeStateService();

        SaveExecutor executor = new SaveExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService
        );

        SaveJob job = new SaveJob
        {
            Name = "Job1",
            Type = SaveType.Full
        };

        // Act + Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync(job, null, CancellationToken.None)
        );

        Assert.AreEqual(2, stateService.States.Count);
        Assert.AreEqual("Active", stateService.States[0].Status);
        Assert.AreEqual("Error", stateService.States[1].Status);
        Assert.AreEqual(0, stateService.States[1].ProgressPercent);
    }

    [TestMethod]
    public async Task ExecuteAllAsync_ShouldExecuteAllJobsSequentially()
    {
        // Arrange
        FakeSaveStrategy fullStrategy = new FakeSaveStrategy();
        FakeSaveStrategy differentialStrategy = new FakeSaveStrategy();
        FakeLogger logger = new FakeLogger();
        FakeStateService stateService = new FakeStateService();

        SaveExecutor executor = new SaveExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService
        );

        List<SaveJob> jobs = new List<SaveJob>
        {
            new SaveJob { Name = "Job1", Type = SaveType.Full },
            new SaveJob { Name = "Job2", Type = SaveType.Differential },
            new SaveJob { Name = "Job3", Type = SaveType.Full }
        };

        // Act
        await executor.ExecuteAllAsync(jobs, null, CancellationToken.None);

        // Assert
        Assert.AreEqual(2, fullStrategy.ExecutionCount);
        Assert.AreEqual(1, differentialStrategy.ExecutionCount);
    }

    private class FakeSaveStrategy : ISaveStrategy
    {
        public int ExecutionCount { get; private set; }
        public bool ShouldThrowException { get; set; }

        public void ExecuteSaveJob(SaveJob job, CancellationToken cancellationToken = default)
        {
            ExecutionCount++;

            if (ShouldThrowException)
            {
                throw new InvalidOperationException("Fake strategy error");
            }
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

    private class FakeLogger : ILogger
    {
        public List<LogEntry> Entries { get; } = new List<LogEntry>();

        public void Log(LogEntry entry)
        {
            Entries.Add(entry);
        }
    }
}