namespace EasyLog
{
    /// <summary>
    /// Contrat minimal pour l'écriture de logs de transfert.
    /// Les implémentations concrètes (JSON, XML) sont dans EasyLog.
    /// EasySave.Core dépend uniquement de cette interface.
    /// </summary>
    public interface ILogger
    {
        void Log(LogEntry entry);
    }
}
