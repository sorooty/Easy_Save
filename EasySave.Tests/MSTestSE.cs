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
        FakeSaveStrategy fullStrategy = new FakeSaveStrategy();
        FakeSaveStrategy differentialStrategy = new FakeSaveStrategy();
        FakeLogger logger = new FakeLogger();
        FakeStateService stateService = new FakeStateService();

        SaveExecutor executor = CreateExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService);

        SaveJob job = new SaveJob
        {
            Name = "Job1",
            Type = SaveType.Full
        };

        await executor.ExecuteAsync(job, null, CancellationToken.None);

        Assert.AreEqual(1, fullStrategy.ExecutionCount);
        Assert.AreEqual(0, differentialStrategy.ExecutionCount);
    }

    [TestMethod]
    public async Task ExecuteAsync_ShouldUseDifferentialStrategy_WhenJobTypeIsDifferential()
    {
        FakeSaveStrategy fullStrategy = new FakeSaveStrategy();
        FakeSaveStrategy differentialStrategy = new FakeSaveStrategy();
        FakeLogger logger = new FakeLogger();
        FakeStateService stateService = new FakeStateService();

        SaveExecutor executor = CreateExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService);

        SaveJob job = new SaveJob
        {
            Name = "Job1",
            Type = SaveType.Differential
        };

        await executor.ExecuteAsync(job, null, CancellationToken.None);

        Assert.AreEqual(0, fullStrategy.ExecutionCount);
        Assert.AreEqual(1, differentialStrategy.ExecutionCount);
    }

    [TestMethod]
    public async Task ExecuteAsync_ShouldUpdateStateToActiveThenCompleted_WhenExecutionSucceeds()
    {
        FakeSaveStrategy fullStrategy = new FakeSaveStrategy();
        FakeSaveStrategy differentialStrategy = new FakeSaveStrategy();
        FakeLogger logger = new FakeLogger();
        FakeStateService stateService = new FakeStateService();

        SaveExecutor executor = CreateExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService);

        SaveJob job = new SaveJob
        {
            Name = "Job1",
            Type = SaveType.Full
        };

        await executor.ExecuteAsync(job, null, CancellationToken.None);

        Assert.IsTrue(stateService.States.Count >= 1);

        SaveState finalState = stateService.States.Last();

        Assert.AreEqual("Job1", finalState.Name);
        Assert.AreEqual("Completed", finalState.Status);
        Assert.AreEqual(100, finalState.ProgressPercent);
    }

    [TestMethod]
    public async Task ExecuteAsync_ShouldUpdateStateToError_WhenStrategyThrowsException()
    {
        FakeSaveStrategy fullStrategy = new FakeSaveStrategy
        {
            ShouldThrowException = true
        };

        FakeSaveStrategy differentialStrategy = new FakeSaveStrategy();
        FakeLogger logger = new FakeLogger();
        FakeStateService stateService = new FakeStateService();

        SaveExecutor executor = CreateExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService);

        SaveJob job = new SaveJob
        {
            Name = "Job1",
            Type = SaveType.Full
        };

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync(job, null, CancellationToken.None));

        Assert.IsTrue(stateService.States.Count >= 1);

        SaveState finalState = stateService.States.Last();

        Assert.AreEqual("Job1", finalState.Name);
        Assert.AreEqual("Error", finalState.Status);
        Assert.AreEqual(0, finalState.ProgressPercent);
    }

    [TestMethod]
    public async Task ExecuteAllAsync_ShouldExecuteAllJobsSequentially()
    {
        FakeSaveStrategy fullStrategy = new FakeSaveStrategy();
        FakeSaveStrategy differentialStrategy = new FakeSaveStrategy();
        FakeLogger logger = new FakeLogger();
        FakeStateService stateService = new FakeStateService();

        SaveExecutor executor = CreateExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService);

        List<SaveJob> jobs = new List<SaveJob>
        {
            new SaveJob { Name = "Job1", Type = SaveType.Full },
            new SaveJob { Name = "Job2", Type = SaveType.Differential },
            new SaveJob { Name = "Job3", Type = SaveType.Full }
        };

        await executor.ExecuteAllAsync(jobs, null, CancellationToken.None);

        Assert.AreEqual(2, fullStrategy.ExecutionCount);
        Assert.AreEqual(1, differentialStrategy.ExecutionCount);
    }

    private static SaveExecutor CreateExecutor(
        ISaveStrategy fullStrategy,
        ISaveStrategy differentialStrategy,
        ILogger logger,
        IStateService stateService)
    {
        var settings = new GeneralSettings
        {
            PriorityExtensions = new List<string> { ".txt" },
            LargeFileLimitKo = 10000
        };

        var priorityFileService = new PriorityFileService(settings);
        var largeFileTransferService = new LargeFileTransferService(settings);

        return new SaveExecutor(
            fullStrategy,
            differentialStrategy,
            logger,
            stateService,
            priorityFileService,
            largeFileTransferService);
    }

    private class FakeSaveStrategy : ISaveStrategy
    {
        public int ExecutionCount { get; private set; }
        public bool ShouldThrowException { get; set; }

        public void ExecuteSaveJob(
    SaveJob job,
    CancellationToken cancellationToken = default,
    IProgress<SaveState>? progress = null,
    PriorityFileService? priorityFileService = null,
    LargeFileTransferService? largeFileTransferService = null)
        {
            ExecutionCount++;

            if (ShouldThrowException)
            {
                throw new InvalidOperationException("Simulated strategy exception");
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