namespace DellinDictionary.Configuration;

public sealed class ImportOptions
{
    public const string SectionName = "Import";

    /// <summary>Время запуска импорта по МСК (HH:mm:ss).</summary>
    public TimeSpan ScheduleTimeMsk { get; init; }

    /// <summary>Максимальное время одного прогона импорта.</summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>Путь к JSON-файлу с терминалами (относительно ContentRoot или абсолютный).</summary>
    public string FilePath { get; init; } = "files/terminals.json";
}
