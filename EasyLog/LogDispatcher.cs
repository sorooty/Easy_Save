using System.Runtime.CompilerServices;

namespace EasyLog;

public class LogDispatcher
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

    public async Task WriteLog(LogEntry logEntry)
    {
        switch (_storageMode)
        {
            case LogStorageMode.localOnly:
                _localLogger.Log(logEntry);
                break;

            case LogStorageMode.CentralOnly:
                await _centralizedLogger.WriteLog(logEntry);
                break;

            case LogStorageMode.LocalAndCentral:
                _localLogger.Log(logEntry);
                await _centralizedLogger.WriteLog(logEntry);
                break;
        }
    }
}
