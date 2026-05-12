using EasyLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests;

[TestClass]
public class LoggerFactoryTests
{
    [TestMethod]
    public void CreateLogger_ShouldReturnJsonLogger_WhenFormatIsJson()
    {
        // Act
        ILogger logger = LoggerFactory.CreateLogger(
            LogFormat.JSON,
            "Logs"
        );

        // Assert
        Assert.IsInstanceOfType(logger, typeof(JsonLogger));
    }

    [TestMethod]
    public void CreateLogger_ShouldReturnXmlLogger_WhenFormatIsXml()
    {
        // Act
        ILogger logger = LoggerFactory.CreateLogger(
            LogFormat.XML,
            "Logs"
        );

        // Assert
        Assert.IsInstanceOfType(logger, typeof(XmlLogger));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateLogger_ShouldThrowException_WhenFormatIsInvalid()
    {
        // Act
        LoggerFactory.CreateLogger(
            (LogFormat)999,
            "Logs"
        );
    }
}