using EasySave.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class RelayCommandTests
{
    [TestMethod]
    public void Execute_ShouldCallAction()
    {
        // Arrange
        bool executed = false;

        RelayCommand command = new RelayCommand(_ =>
        {
            executed = true;
        });

        // Act
        command.Execute(null);

        // Assert
        Assert.IsTrue(executed);
    }

    [TestMethod]
    public void CanExecute_ShouldReturnTrue_WhenNoPredicateProvided()
    {
        // Arrange
        RelayCommand command = new RelayCommand(_ => { });

        // Act
        bool result = command.CanExecute(null);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CanExecute_ShouldUsePredicateResult()
    {
        // Arrange
        RelayCommand command = new RelayCommand(
            _ => { },
            _ => false
        );

        // Act
        bool result = command.CanExecute(null);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void RaiseCanExecuteChanged_ShouldRaiseEvent()
    {
        // Arrange
        RelayCommand command = new RelayCommand(_ => { });

        bool eventRaised = false;

        command.CanExecuteChanged += (_, _) =>
        {
            eventRaised = true;
        };

        // Act
        command.RaiseCanExecuteChanged();

        // Assert
        Assert.IsTrue(eventRaised);
    }

    [TestMethod]
    public void GenericExecute_ShouldPassParameter()
    {
        // Arrange
        string? receivedValue = null;

        RelayCommand<string> command =
            new RelayCommand<string>(value =>
            {
                receivedValue = value;
            });

        // Act
        command.Execute("Hello");

        // Assert
        Assert.AreEqual("Hello", receivedValue);
    }

    [TestMethod]
    public void GenericCanExecute_ShouldUsePredicate()
    {
        // Arrange
        RelayCommand<string> command =
            new RelayCommand<string>(
                _ => { },
                value => value == "OK"
            );

        // Act
        bool result1 = command.CanExecute("OK");
        bool result2 = command.CanExecute("NO");

        // Assert
        Assert.IsTrue(result1);
        Assert.IsFalse(result2);
    }

    [TestMethod]
    public void GenericRaiseCanExecuteChanged_ShouldRaiseEvent()
    {
        // Arrange
        RelayCommand<string> command =
            new RelayCommand<string>(_ => { });

        bool eventRaised = false;

        command.CanExecuteChanged += (_, _) =>
        {
            eventRaised = true;
        };

        // Act
        command.RaiseCanExecuteChanged();

        // Assert
        Assert.IsTrue(eventRaised);
    }
}