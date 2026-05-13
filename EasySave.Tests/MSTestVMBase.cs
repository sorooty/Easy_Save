using EasySave.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace EasySave.Tests;

[TestClass]
public class ViewModelBaseTests
{
    [TestMethod]
    public void Set_ShouldUpdateValue_WhenValueChanges()
    {
        // Arrange
        TestViewModel vm = new TestViewModel();

        // Act
        vm.TestProperty = "Hello";

        // Assert
        Assert.AreEqual("Hello", vm.TestProperty);
    }

    [TestMethod]
    public void Set_ShouldReturnTrue_WhenValueChanges()
    {
        // Arrange
        TestViewModel vm = new TestViewModel();

        // Act
        bool result = vm.SetProperty("Hello");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Set_ShouldReturnFalse_WhenValueDoesNotChange()
    {
        // Arrange
        TestViewModel vm = new TestViewModel();
        vm.TestProperty = "Hello";

        // Act
        bool result = vm.SetProperty("Hello");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Set_ShouldRaisePropertyChangedEvent()
    {
        // Arrange
        TestViewModel vm = new TestViewModel();

        bool eventRaised = false;

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TestViewModel.TestProperty))
            {
                eventRaised = true;
            }
        };

        // Act
        vm.TestProperty = "Hello";

        // Assert
        Assert.IsTrue(eventRaised);
    }

    [TestMethod]
    public void Set_ShouldRaiseCorrectPropertyName()
    {
        // Arrange
        TestViewModel vm = new TestViewModel();

        string? receivedPropertyName = null;

        vm.PropertyChanged += (_, e) =>
        {
            receivedPropertyName = e.PropertyName;
        };

        // Act
        vm.TestProperty = "Hello";

        // Assert
        Assert.AreEqual(nameof(TestViewModel.TestProperty), receivedPropertyName);
    }

    private class TestViewModel : ViewModelBase
    {
        private string _testProperty = string.Empty;

        public string TestProperty
        {
            get => _testProperty;
            set => Set(ref _testProperty, value);
        }

        public bool SetProperty(string value)
        {
            return Set(ref _testProperty, value, nameof(TestProperty));
        }
    }
}