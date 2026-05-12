using EasySave.Core.Model.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace EasySave.Tests;

[TestClass]
public class BusinessSoftwareServiceTests
{
    [TestMethod]
    public void IsBusinessSoftwareRunning_ShouldReturnFalse_WhenNameIsNull()
    {
        // Arrange
        BusinessSoftwareService service = new BusinessSoftwareService();

        // Act
        bool result = service.IsBusinessSoftwareRunning(null!);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsBusinessSoftwareRunning_ShouldReturnFalse_WhenNameIsEmpty()
    {
        // Arrange
        BusinessSoftwareService service = new BusinessSoftwareService();

        // Act
        bool result = service.IsBusinessSoftwareRunning("");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsBusinessSoftwareRunning_ShouldReturnFalse_WhenProcessDoesNotExist()
    {
        // Arrange
        BusinessSoftwareService service = new BusinessSoftwareService();

        // Act
        bool result = service.IsBusinessSoftwareRunning("ProcessThatDoesNotExist123");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsBusinessSoftwareRunning_ShouldReturnTrue_WhenProcessExists()
    {
        // Arrange
        BusinessSoftwareService service = new BusinessSoftwareService();

        Process process = Process.Start("notepad.exe")!;

        try
        {
            // Act
            bool result = service.IsBusinessSoftwareRunning("notepad");

            // Assert
            Assert.IsTrue(result);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
        }
    }
}