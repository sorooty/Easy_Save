using System.Runtime.CompilerServices;

namespace EasyLog;

public class LogDispatcher: ILogger
{
    private readonly ILogger _localLogger;
    private readonly CentralizedLogger _centralizedLogger;
    private readonly LogStorageMode _storageMode;

    // Contructeur pour initialiser les dépendances du LogDispatcher
    public LogDispatcher(ILogger localLogger, CentralizedLogger centralizedLogger, LogStorageMode storageMode)
    {
        _localLogger = localLogger;
        _centralizedLogger = centralizedLogger;
        _storageMode = storageMode;
    }

    /// <summary>
    /// se charge de dispatcher les logs vers les destinations appropriées en fonction du mode de stockage configuré.
    /// </summary>
    /// <param name="logEntry">Entrée de journal à dispatcher.</param>
    public void Log(LogEntry logEntry)
    {
        switch (_storageMode)
        {
            case LogStorageMode.LocalOnly:
                _localLogger.Log(logEntry);
                break;

            case LogStorageMode.CentralOnly:
                _centralizedLogger.WriteLog(logEntry).GetAwaiter().GetResult();
                break;

            case LogStorageMode.LocalAndCentral:
                _localLogger.Log(logEntry);
                _centralizedLogger.WriteLog(logEntry).GetAwaiter().GetResult();
                break;
        }
    }
}
