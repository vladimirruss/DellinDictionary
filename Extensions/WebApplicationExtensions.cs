using DellinDictionary.Data;
using Microsoft.EntityFrameworkCore;

namespace DellinDictionary.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<DellinDictionaryDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DellinDictionary.Migrations");
        var ct = app.Lifetime.ApplicationStopping;

        try
        {
            var pending = (await db.Database.GetPendingMigrationsAsync(ct)).ToList();
            if (pending.Count == 0)
            {
                logger.LogInformation("Миграции БД: изменений нет");
                return;
            }

            logger.LogInformation("Применяю миграции ({Count}): {Migrations}",
                pending.Count, string.Join(", ", pending));
            await db.Database.MigrateAsync(ct);
            logger.LogInformation("Миграции применены");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Ошибка применения миграций");
            throw;
        }
    }
}
