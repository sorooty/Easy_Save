using EasyLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;

namespace EasySave.Tests;

[TestClass]
public class XmlLoggerTests
{
    private string _testLogDirectory = string.Empty;

    [TestInitialize]
    public void Setup()
    {
        _testLogDirectory = Path.Combine(
            Path.GetTempPath(),
            "XmlLoggerTests",
            Guid.NewGuid().ToString()
        );
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
    public void Constructor_ShouldCreateDirectory_WhenDirectoryDoesNotExist()
    {
        // Act
        XmlLogger logger = new XmlLogger(_testLogDirectory);

        // Assert
        Assert.IsTrue(Directory.Exists(_testLogDirectory));
    }

    [TestMethod]
    public void Log_ShouldCreateXmlFile_WhenFileDoesNotExist()
    {
        // Arrange
        XmlLogger logger = new XmlLogger(_testLogDirectory);

        LogEntry entry = new LogEntry(
            "Job1",
            @"C:\Source\file.txt",
            @"D:\Backup\file.txt",
            100,
            10,
            "Success",
            ""
        );

        string expectedFilePath = Path.Combine(
            _testLogDirectory,
            $"{DateTime.Now:yyyy-MM-dd}.xml"
        );

        // Act
        logger.Log(entry);

        // Assert
        Assert.IsTrue(File.Exists(expectedFilePath));
    }

    [TestMethod]
    public void Log_ShouldWriteEntryIntoXmlFile()
    {
        // Arrange
        XmlLogger logger = new XmlLogger(_testLogDirectory);

        LogEntry entry = new LogEntry(
            "Job1",
            @"C:\Source\file.txt",
            @"D:\Backup\file.txt",
            100,
            10,
            "Success",
            "",
            5
        );

        string filePath = Path.Combine(
            _testLogDirectory,
            $"{DateTime.Now:yyyy-MM-dd}.xml"
        );

        XmlSerializer serializer =
            new XmlSerializer(typeof(List<LogEntry>));

        // Act
        logger.Log(entry);

        // Assert
        List<LogEntry>? logs;

        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        {
            logs = serializer.Deserialize(fs) as List<LogEntry>;
        }

        Assert.IsNotNull(logs);
        Assert.AreEqual(1, logs.Count);

        Assert.AreEqual("Job1", logs[0].JobName);
        Assert.AreEqual(@"C:\Source\file.txt", logs[0].SourceFile);
        Assert.AreEqual(@"D:\Backup\file.txt", logs[0].TargetFile);
        Assert.AreEqual(100, logs[0].FileSizeBytes);
        Assert.AreEqual(10, logs[0].TransferDurationMs);
        Assert.AreEqual("Success", logs[0].State);
        Assert.AreEqual("", logs[0].ErrorMessage);
        Assert.AreEqual(5, logs[0].EncryptionTimeMs);
    }

    [TestMethod]
    public void Log_ShouldAppendEntry_WhenFileAlreadyExists()
    {
        // Arrange
        XmlLogger logger = new XmlLogger(_testLogDirectory);

        LogEntry firstEntry = new LogEntry(
            "Job1",
            @"C:\file1.txt",
            @"D:\backup1.txt",
            100,
            10,
            "Success",
            ""
        );

        LogEntry secondEntry = new LogEntry(
            "Job2",
            @"C:\file2.txt",
            @"D:\backup2.txt",
            200,
            20,
            "Success",
            ""
        );

        string filePath = Path.Combine(
            _testLogDirectory,
            $"{DateTime.Now:yyyy-MM-dd}.xml"
        );

        XmlSerializer serializer =
            new XmlSerializer(typeof(List<LogEntry>));

        // Act
        logger.Log(firstEntry);
        logger.Log(secondEntry);

        // Assert
        List<LogEntry>? logs;

        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        {
            logs = serializer.Deserialize(fs) as List<LogEntry>;
        }

        Assert.IsNotNull(logs);
        Assert.AreEqual(2, logs.Count);

        Assert.AreEqual("Job1", logs[0].JobName);
        Assert.AreEqual("Job2", logs[1].JobName);
    }

    [TestMethod]
    public void Log_ShouldCreateNewList_WhenXmlContentIsInvalid()
    {
        // Arrange
        XmlLogger logger = new XmlLogger(_testLogDirectory);

        string filePath = Path.Combine(
            _testLogDirectory,
            $"{DateTime.Now:yyyy-MM-dd}.xml"
        );

        File.WriteAllText(filePath, "INVALID XML");

        LogEntry entry = new LogEntry(
            "Job1",
            @"C:\Source\file.txt",
            @"D:\Backup\file.txt",
            100,
            10,
            "Success",
            ""
        );

        XmlSerializer serializer =
            new XmlSerializer(typeof(List<LogEntry>));

        // Act
        logger.Log(entry);

        // Assert
        List<LogEntry>? logs;

        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        {
            logs = serializer.Deserialize(fs) as List<LogEntry>;
        }

        Assert.IsNotNull(logs);
        Assert.AreEqual(1, logs.Count);
        Assert.AreEqual("Job1", logs[0].JobName);
    }
}