namespace EasyLog;

public static class LoggerFactory
{
    /// <summary>
    /// Crée une instance de <see cref="ILogger"/> basée sur le format spécifié.
    /// </summary>
    /// <param name="format">Le format de log souhaité (Json ou Xml).</param>
    /// <param name="logDirectory">Le répertoire où les fichiers de log seront stockés.</param>
    /// <returns>Une instance de <see cref="ILogger"/> correspondant au format demandé.</returns>
    /// <exception cref="ArgumentException">Levée si le format spécifié n'est pas supporté.</exception>
    public static ILogger CreateLogger(LogFormat format, string logDirectory)
    {
        return format switch
        {
            LogFormat.JSON => new JsonLogger(logDirectory),
            LogFormat.XML => new XmlLogger(logDirectory),
            _ => throw new ArgumentException($"Format de log non supporté: {format}")
        };
    }
}