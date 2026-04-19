using DellinDictionary.Configuration;
using DellinDictionary.Helpers;
using DellinDictionary.Services;
using Microsoft.Extensions.Options;

namespace DellinDictionary;

public class ImportSchedulerWorker : BackgroundService
{
    private readonly IImportService _importService;
    private readonly ILogger<ImportSchedulerWorker> _logger;
    private readonly ImportOptions _options;

    public ImportSchedulerWorker(
        IImportService importService,
        ILogger<ImportSchedulerWorker> logger,
        IOptions<ImportOptions> options)
    {
        _importService = importService;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ImportSchedulerWorker запущен");

        await _importService.RunAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = ScheduleHelper.GetDelayUntilNextRun(_options.ScheduleTimeMsk);
            _logger.LogInformation("Следующий импорт через {Delay}", delay);

            await Task.Delay(delay, stoppingToken)
                .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Пропускаем запуск импорта из-за shutdown");
                break;
            }

            await _importService.RunAsync(stoppingToken);
        }

        _logger.LogInformation("ImportSchedulerWorker остановлен (graceful shutdown)");
    }
}
