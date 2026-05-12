using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class SettingsServiceTests
{
    private string _testDirectory = string.Empty;
    private FakePathService _paths = null!;
    private SettingsService _settingsService = null!;

    [TestInitialize]
    public void Setup()
    {
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            "SettingsServiceTests",
            Guid.NewGuid().ToString()
        );

        Directory.CreateDirectory(_testDirectory);

        _paths = new FakePathService(_testDirectory);
        _settingsService = new SettingsService(_paths);
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
    public void LoadSettings_ShouldReturnDefaultSettings_WhenFileDoesNotExist()
    {
        // Act
        GeneralSettings settings = _settingsService.LoadSettings();

        // Assert
        Assert.AreEqual("en", settings.Language);
        Assert.AreEqual(EasyLog.LogFormat.JSON, settings.LogFormat);
        Assert.AreEqual(string.Empty, settings.BusinessSoftwareName);
        Assert.AreEqual(0, settings.EncryptedExtensions.Count);
    }

    [TestMethod]
    public void SaveSettings_ShouldCreateSettingsFile()
    {
        // Arrange
        GeneralSettings settings = new GeneralSettings
        {
            Language = "fr"
        };

        // Act
        _settingsService.SaveSettings(settings);

        // Assert
        Assert.IsTrue(File.Exists(_paths.SettingsFile));
    }

    [TestMethod]
    public void LoadSettings_ShouldLoadSavedSettings()
    {
        // Arrange
        GeneralSettings settings = new GeneralSettings
        {
            Language = "fr",
            BusinessSoftwareName = "calc",
            CryptoSoftPath = @"C:\CryptoSoft.exe",
            LogFormat = EasyLog.LogFormat.XML,
            EncryptedExtensions = new List<string>
            {
                ".docx",
                ".xlsx"
            }
        };

        _settingsService.SaveSettings(settings);

        // Act
        GeneralSettings loadedSettings =
            _settingsService.LoadSettings();

        // Assert
        Assert.AreEqual("fr", loadedSettings.Language);
        Assert.AreEqual("calc", loadedSettings.BusinessSoftwareName);
        Assert.AreEqual(
            @"C:\CryptoSoft.exe",
            loadedSettings.CryptoSoftPath
        );

        Assert.AreEqual(
            EasyLog.LogFormat.XML,
            loadedSettings.LogFormat
        );

        Assert.AreEqual(2, loadedSettings.EncryptedExtensions.Count);
        Assert.AreEqual(".docx", loadedSettings.EncryptedExtensions[0]);
        Assert.AreEqual(".xlsx", loadedSettings.EncryptedExtensions[1]);
    }

    [TestMethod]
    public void LoadSettings_ShouldReturnDefaultSettings_WhenJsonIsInvalid()
    {
        // Arrange
        File.WriteAllText(_paths.SettingsFile, "INVALID JSON");

        // Act
        GeneralSettings settings =
            _settingsService.LoadSettings();

        // Assert
        Assert.AreEqual("en", settings.Language);
        Assert.AreEqual(EasyLog.LogFormat.JSON, settings.LogFormat);
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