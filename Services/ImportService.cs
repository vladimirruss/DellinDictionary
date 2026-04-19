using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using DellinDictionary.Configuration;
using DellinDictionary.Data;
using DellinDictionary.Mappers;
using DellinDictionary.Models.Entities;
using DellinDictionary.Models.Import;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DellinDictionary.Services;

public class ImportService : IImportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IDbContextFactory<DellinDictionaryDbContext> _dbFactory;
    private readonly ILogger<ImportService> _logger;
    private readonly IHostEnvironment _env;
    private readonly ImportOptions _options;

    public ImportService(
        IDbContextFactory<DellinDictionaryDbContext> dbFactory,
        ILogger<ImportService> logger,
        IHostEnvironment env,
        IOptions<ImportOptions> options)
    {
        _dbFactory = dbFactory;
        _logger = logger;
        _env = env;
        _options = options.Value;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(_options.Timeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await ImportAsync(linked.Token);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Импорт остановлен из-за shutdown");
            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Импорт прерван по таймауту ({Timeout})", _options.Timeout);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Ошибка записи в БД при импорте");
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "Ошибка БД при импорте");
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Ошибка ввода/вывода при импорте");
        }
    }

    private async Task ImportAsync(CancellationToken cancellationToken)
    {
        var offices = await ReadOfficesAsync(cancellationToken);
        if (offices is null) return;

        var saved = await ReplaceOfficesAsync(offices, cancellationToken);
        _logger.LogInformation("Импорт завершён: сохранено {Count} терминалов", saved);
    }

    private async Task<List<Office>?> ReadOfficesAsync(CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(_env.ContentRootPath, _options.FilePath);

        try
        {
            await using var stream = new FileStream(
                filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true);

            var offices = new List<Office>();
            await foreach (var import in JsonSerializer.DeserializeAsyncEnumerable<OfficeImport>(
                stream, JsonOptions, cancellationToken))
            {
                if (import is not null)
                    offices.Add(import.ToOffice());
            }

            if (offices.Count == 0)
            {
                _logger.LogWarning("Файл пустой, импорт пропущен: {FilePath}", filePath);
                return null;
            }

            _logger.LogInformation("Прочитано {Count} терминалов из файла", offices.Count);
            return offices;
        }
        catch (FileNotFoundException)
        {
            _logger.LogError("Файл не найден: {FilePath}", filePath);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Ошибка парсинга JSON: {FilePath}", filePath);
            return null;
        }
    }

    private async Task<int> ReplaceOfficesAsync(List<Office> offices, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        await db.Offices.ExecuteDeleteAsync(cancellationToken);

        db.ChangeTracker.AutoDetectChangesEnabled = false;
        db.Offices.AddRange(offices);
        await db.SaveChangesAsync(cancellationToken);

        await tx.CommitAsync(cancellationToken);

        return offices.Count;
    }
}
