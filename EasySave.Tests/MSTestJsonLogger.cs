using EasyLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace EasyLog.Tests;

[TestClass]
public class JsonLoggerTests
{
    private string _testLogDirectory = string.Empty;

    [TestInitialize]
    public void Setup()
    {
        _testLogDirectory = Path.Combine(Path.GetTempPath(), "EasyLogTests", Guid.NewGuid().ToString());
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_testLogDirectory))
        {
            Directory.Delete(_testLogDirectory, true);
        }
    }

    [TestMethod]
    public void Constructor_ShouldCreateLogDirectory_WhenDirectoryDoesNotExist()
    {
        // Act
        JsonLogger logger = new JsonLogger(_testLogDirectory);

        // Assert
        Assert.IsTrue(Directory.Exists(_testLogDirectory));
    }

    [TestMethod]
    public void Log_ShouldCreateJsonFile_WhenFileDoesNotExist()
    {
        // Arrange
        JsonLogger logger = new JsonLogger(_testLogDirectory);

        LogEntry entry = new LogEntry(
            "Job1",
            @"C:\Source\file.txt",
            @"D:\Backup\file.txt",
            1024,
            50,
            "Success",
            ""
        );

        string expectedFilePath = Path.Combine(
            _testLogDirectory,
            $"{DateTime.Now:yyyy-MM-dd}.json"
        );

        // Act
        logger.Log(entry);

        // Assert
        Assert.IsTrue(File.Exists(expectedFilePath));
    }

    [TestMethod]
    public void Log_ShouldWriteLogEntryIntoJsonFile()
    {
        // Arrange
        JsonLogger logger = new JsonLogger(_testLogDirectory);

        LogEntry entry = new LogEntry(
            "Job1",
            @"C:\Source\file.txt",
            @"D:\Backup\file.txt",
            1024,
            50,
            "Success",
            "",
            12
        );

        string filePath = Path.Combine(
            _testLogDirectory,
            $"{DateTime.Now:yyyy-MM-dd}.json"
        );

        // Act
        logger.Log(entry);

        // Assert
        string json = File.ReadAllText(filePath);
        List<LogEntry>? logs = JsonSerializer.Deserialize<List<LogEntry>>(json);

        Assert.IsNotNull(logs);
        Assert.AreEqual(1, logs.Count);

        Assert.AreEqual("Job1", logs[0].JobName);
        Assert.AreEqual(@"C:\Source\file.txt", logs[0].SourceFile);
        Assert.AreEqual(@"D:\Backup\file.txt", logs[0].TargetFile);
        Assert.AreEqual(1024, logs[0].FileSizeBytes);
        Assert.AreEqual(50, logs[0].TransferDurationMs);
        Assert.AreEqual("Success", logs[0].State);
        Assert.AreEqual("", logs[0].ErrorMessage);
        Assert.AreEqual(12, logs[0].EncryptionTimeMs);
    }

    [TestMethod]
    public void Log_ShouldAppendEntry_WhenFileAlreadyExists()
    {
        // Arrange
        JsonLogger logger = new JsonLogger(_testLogDirectory);

        LogEntry firstEntry = new LogEntry(
            "Job1",
            @"C:\Source\file1.txt",
            @"D:\Backup\file1.txt",
            100,
            10,
            "Success",
            ""
        );

        LogEntry secondEntry = new LogEntry(
            "Job2",
            @"C:\Source\file2.txt",
            @"D:\Backup\file2.txt",
            200,
            20,
            "Success",
            ""
        );

        string filePath = Path.Combine(
            _testLogDirectory,
            $"{DateTime.Now:yyyy-MM-dd}.json"
        );

        // Act
        logger.Log(firstEntry);
        logger.Log(secondEntry);

        // Assert
        string json = File.ReadAllText(filePath);
        List<LogEntry>? logs = JsonSerializer.Deserialize<List<LogEntry>>(json);

        Assert.IsNotNull(logs);
        Assert.AreEqual(2, logs.Count);

        Assert.AreEqual("Job1", logs[0].JobName);
        Assert.AreEqual("Job2", logs[1].JobName);
    }

    [TestMethod]
    public void Log_ShouldCreateNewList_WhenJsonFileIsEmpty()
    {
        // Arrange
        JsonLogger logger = new JsonLogger(_testLogDirectory);

        string filePath = Path.Combine(
            _testLogDirectory,
            $"{DateTime.Now:yyyy-MM-dd}.json"
        );

        File.WriteAllText(filePath, "");

        LogEntry entry = new LogEntry(
            "Job1",
            @"C:\Source\file.txt",
            @"D:\Backup\file.txt",
            1024,
            50,
            "Success",
            ""
        );

        // Act
        logger.Log(entry);

        // Assert
        string json = File.ReadAllText(filePath);
        List<LogEntry>? logs = JsonSerializer.Deserialize<List<LogEntry>>(json);

        Assert.IsNotNull(logs);
        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("Job1", logs[0].JobName);
    }
}