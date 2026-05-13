using EasyLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyLog.Tests;

[TestClass]
public class LogEntryTests
{
    [TestMethod]
    public void DefaultConstructor_ShouldInitializeStringPropertiesToEmpty()
    {
        // Act
        LogEntry entry = new LogEntry();

        // Assert
        Assert.AreEqual(string.Empty, entry.JobName);
        Assert.AreEqual(string.Empty, entry.SourceFile);
        Assert.AreEqual(string.Empty, entry.TargetFile);
        Assert.AreEqual(string.Empty, entry.State);
        Assert.AreEqual(string.Empty, entry.ErrorMessage);
    }

    [TestMethod]
    public void DefaultConstructor_ShouldKeepNumericPropertiesAtDefaultValue()
    {
        // Act
        LogEntry entry = new LogEntry();

        // Assert
        Assert.AreEqual(0, entry.FileSizeBytes);
        Assert.AreEqual(0, entry.TransferDurationMs);
        Assert.AreEqual(0, entry.EncryptionTimeMs);
    }

    [TestMethod]
    public void ParameterizedConstructor_ShouldInitializeAllProperties()
    {
        // Act
        LogEntry entry = new LogEntry(
            "Job1",
            @"C:\Source\file.txt",
            @"D:\Backup\file.txt",
            1024,
            25,
            "Success",
            "",
            12
        );

        // Assert
        Assert.AreEqual("Job1", entry.JobName);
        Assert.AreEqual(@"C:\Source\file.txt", entry.SourceFile);
        Assert.AreEqual(@"D:\Backup\file.txt", entry.TargetFile);
        Assert.AreEqual(1024, entry.FileSizeBytes);
        Assert.AreEqual(25, entry.TransferDurationMs);
        Assert.AreEqual("Success", entry.State);
        Assert.AreEqual("", entry.ErrorMessage);
        Assert.AreEqual(12, entry.EncryptionTimeMs);
    }

    [TestMethod]
    public void ParameterizedConstructor_ShouldSetTimeStampToCurrentTime()
    {
        // Arrange
        DateTime beforeCreation = DateTime.Now;

        // Act
        LogEntry entry = new LogEntry(
            "Job1",
            @"C:\Source\file.txt",
            @"D:\Backup\file.txt",
            1024,
            25,
            "Success",
            ""
        );

        DateTime afterCreation = DateTime.Now;

        // Assert
        Assert.IsTrue(entry.TimeStamp >= beforeCreation);
        Assert.IsTrue(entry.TimeStamp <= afterCreation);
    }

    [TestMethod]
    public void ParameterizedConstructor_ShouldSetEncryptionTimeMsToZero_WhenNotProvided()
    {
        // Act
        LogEntry entry = new LogEntry(
            "Job1",
            @"C:\Source\file.txt",
            @"D:\Backup\file.txt",
            1024,
            25,
            "Success",
            ""
        );

        // Assert
        Assert.AreEqual(0, entry.EncryptionTimeMs);
    }
}