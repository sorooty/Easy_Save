using EasyLog;
using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using EasySave.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class SettingsViewModelTests
{
    private string _testDirectory = string.Empty;
    private FakePathService _paths = null!;
    private SettingsService _settingsService = null!;
    private LanguageService _languageService = null!;

    [TestInitialize]
    public void Setup()
    {
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            "SettingsViewModelTests",
            Guid.NewGuid().ToString()
        );

        Directory.CreateDirectory(_testDirectory);

        _paths = new FakePathService(_testDirectory);
        _settingsService = new SettingsService(_paths);
        _languageService = new LanguageService();
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
    public void Constructor_ShouldLoadExistingSettings()
    {
        // Arrange
        _settingsService.SaveSettings(new GeneralSettings
        {
            LogFormat = LogFormat.XML,
            Language = "fr",
            BusinessSoftwareName = "calc",
            CryptoSoftPath = @"C:\CryptoSoft.exe",
            EncryptedExtensions = new List<string> { ".docx", ".xlsx" }
        });

        // Act
        SettingsViewModel vm = new SettingsViewModel(_settingsService, _languageService);

        // Assert
        Assert.IsTrue(vm.UseXml);
        Assert.IsFalse(vm.UseJson);
        Assert.IsTrue(vm.UseFrench);
        Assert.IsFalse(vm.UseEnglish);
        Assert.AreEqual("calc", vm.BusinessSoftwareName);
        Assert.AreEqual(@"C:\CryptoSoft.exe", vm.CryptoSoftPath);
        Assert.AreEqual(".docx, .xlsx", vm.EncryptedExtensions);
    }

    [TestMethod]
    public void UseXml_ShouldDisableUseJson()
    {
        // Arrange
        SettingsViewModel vm = CreateViewModel();

        // Act
        vm.UseXml = true;

        // Assert
        Assert.IsTrue(vm.UseXml);
        Assert.IsFalse(vm.UseJson);
    }

    [TestMethod]
    public void UseFrench_ShouldDisableUseEnglish()
    {
        // Arrange
        SettingsViewModel vm = CreateViewModel();

        // Act
        vm.UseFrench = true;

        // Assert
        Assert.IsTrue(vm.UseFrench);
        Assert.IsFalse(vm.UseEnglish);
    }

    [TestMethod]
    public void SaveCommand_ShouldSaveSelectedSettings()
    {
        // Arrange
        SettingsViewModel vm = CreateViewModel();

        vm.UseXml = true;
        vm.UseFrench = true;
        vm.BusinessSoftwareName = "calc";
        vm.CryptoSoftPath = @"C:\CryptoSoft.exe";
        vm.EncryptedExtensions = ".docx, .xlsx";

        // Act
        vm.SaveCommand.Execute(null);

        // Assert
        GeneralSettings saved = _settingsService.LoadSettings();

        Assert.AreEqual(LogFormat.XML, saved.LogFormat);
        Assert.AreEqual("fr", saved.Language);
        Assert.AreEqual("calc", saved.BusinessSoftwareName);
        Assert.AreEqual(@"C:\CryptoSoft.exe", saved.CryptoSoftPath);
        Assert.AreEqual(2, saved.EncryptedExtensions.Count);
        Assert.AreEqual(".docx", saved.EncryptedExtensions[0]);
        Assert.AreEqual(".xlsx", saved.EncryptedExtensions[1]);
    }

    [TestMethod]
    public void SaveCommand_ShouldRemoveEmptyExtensions()
    {
        // Arrange
        SettingsViewModel vm = CreateViewModel();

        vm.EncryptedExtensions = ".docx, , .xlsx, ";

        // Act
        vm.SaveCommand.Execute(null);

        // Assert
        GeneralSettings saved = _settingsService.LoadSettings();

        Assert.AreEqual(2, saved.EncryptedExtensions.Count);
        Assert.AreEqual(".docx", saved.EncryptedExtensions[0]);
        Assert.AreEqual(".xlsx", saved.EncryptedExtensions[1]);
    }

    private SettingsViewModel CreateViewModel()
    {
        return new SettingsViewModel(_settingsService, _languageService);
    }

    private class FakePathService : IPathService
    {
        private readonly string _baseDirectory;

        public FakePathService(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public string JobsFile => Path.Combine(_baseDirectory, "jobs.json");
        public string StateFile => Path.Combine(_baseDirectory, "state.json");
        public string LogsDirectory => Path.Combine(_baseDirectory, "Logs");
        public string SettingsFile => Path.Combine(_baseDirectory, "settings.json");

        public void EnsureDirectoriesExist()
        {
        }
    }
}