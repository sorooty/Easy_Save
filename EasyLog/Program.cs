// Ce fichier est intentionnellement vide — EasyLog est une bibliothèque de classes (DLL).

/* ====================================================================================
                             Manuel Test / Integration Test
====================================================================================== */

using EasyLog;

LogEntry logEntry = new LogEntry(
    jobName: "TestJob",
    sourceFile: "D:\\OneDrive\\CESI\\FISE A3\\6_Genie_Logiciel\\PROJET\\Test",
    targetFile: "D:\\OneDrive\\CESI\\FISE A3\\6_Genie_Logiciel\\PROJET\\TestFinal",
    fileSizeBytes: 1024,
    transferDuration: 500,
    state: "Success",
    errorMessage: string.Empty
);

ILogger localLogger = LoggerFactory.CreateLogger(LogFormat.JSON, logEntry.SourceFile);

CentralizedLogger centralizedLogger = new CentralizedLogger("http://localhost:5000/api/logs");

LogDispatcher logDispatcher = new LogDispatcher(
    localLogger, 
    centralizedLogger, 
    LogStorageMode.LocalAndCentral
    );

await logDispatcher.WriteLog(logEntry);

Console.WriteLine("Log envoyé.");