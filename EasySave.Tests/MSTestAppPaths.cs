using EasySave.Core.Model.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class AppPathsTests
{
    [TestMethod]
    public void JobsFile_ShouldEndWithJobsJson()
    {
        // Arrange
        AppPaths paths = new AppPaths();

        // Assert
        StringAssert.EndsWith(paths.JobsFile, "jobs.json");
    }

    [TestMethod]
    public void StateFile_ShouldEndWithStateJson()
    {
        // Arrange
        AppPaths paths = new AppPaths();

        // Assert
        StringAssert.EndsWith(paths.StateFile, "state.json");
    }

    [TestMethod]
    public void SettingsFile_ShouldEndWithSettingsJson()
    {
        // Arrange
        AppPaths paths = new AppPaths();

        // Assert
        StringAssert.EndsWith(paths.SettingsFile, "settings.json");
    }

    [TestMethod]
    public void LogsDirectory_ShouldContainLogsFolder()
    {
        // Arrange
        AppPaths paths = new AppPaths();

        // Assert
        StringAssert.Contains(paths.LogsDirectory, "Logs");
    }

    [TestMethod]
    public void EnsureDirectoriesExist_ShouldCreateDirectories()
    {
        // Arrange
        AppPaths paths = new AppPaths();

        // Act
        paths.EnsureDirectoriesExist();

        // Assert
        Assert.IsTrue(Directory.Exists(paths.LogsDirectory));
    }
}