using EasySave.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class MainViewModelTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeJobsViewAsCurrentView()
    {
        // Arrange
        SaveJobListViewModel jobsVm = CreateJobsViewModel();
        SettingsViewModel settingsVm = CreateSettingsViewModel();

        // Act
        MainViewModel vm = new MainViewModel(jobsVm, settingsVm);

        // Assert
        Assert.AreEqual(jobsVm, vm.CurrentView);
        Assert.IsTrue(vm.IsJobsPage);
        Assert.IsFalse(vm.IsSettingsPage);
    }

    [TestMethod]
    public void ShowSettingsCommand_ShouldSwitchToSettingsView()
    {
        // Arrange
        SaveJobListViewModel jobsVm = CreateJobsViewModel();
        SettingsViewModel settingsVm = CreateSettingsViewModel();

        MainViewModel vm = new MainViewModel(jobsVm, settingsVm);

        // Act
        vm.ShowSettingsCommand.Execute(null);

        // Assert
        Assert.AreEqual(settingsVm, vm.CurrentView);
        Assert.IsFalse(vm.IsJobsPage);
        Assert.IsTrue(vm.IsSettingsPage);
    }

    [TestMethod]
    public void ShowJobsCommand_ShouldSwitchToJobsView()
    {
        // Arrange
        SaveJobListViewModel jobsVm = CreateJobsViewModel();
        SettingsViewModel settingsVm = CreateSettingsViewModel();

        MainViewModel vm = new MainViewModel(jobsVm, settingsVm);

        vm.ShowSettingsCommand.Execute(null);

        // Act
        vm.ShowJobsCommand.Execute(null);

        // Assert
        Assert.AreEqual(jobsVm, vm.CurrentView);
        Assert.IsTrue(vm.IsJobsPage);
        Assert.IsFalse(vm.IsSettingsPage);
    }

    private SaveJobListViewModel CreateJobsViewModel()
    {
        return null!;
    }

    private SettingsViewModel CreateSettingsViewModel()
    {
        return null!;
    }
}