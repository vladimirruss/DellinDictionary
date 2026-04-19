using DellinDictionary.Configuration;
using DellinDictionary.Data;
using DellinDictionary.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DellinDictionary.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        void Configure(DbContextOptionsBuilder options) =>
            options.UseNpgsql(connectionString)
                   .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

        services.AddDbContext<DellinDictionaryDbContext>(Configure, optionsLifetime: ServiceLifetime.Singleton);
        services.AddDbContextFactory<DellinDictionaryDbContext>(Configure);
        return services;
    }

    public static IServiceCollection AddImportScheduler(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ImportOptions>()
            .Bind(configuration.GetSection(ImportOptions.SectionName))
            .Validate(o => o.ScheduleTimeMsk > TimeSpan.Zero && o.ScheduleTimeMsk < TimeSpan.FromDays(1),
                "Import:ScheduleTimeMsk должен быть в диапазоне 00:00:01..23:59:59")
            .Validate(o => o.Timeout > TimeSpan.Zero,
                "Import:Timeout должен быть положительным")
            .Validate(o => !string.IsNullOrWhiteSpace(o.FilePath),
                "Import:FilePath не должен быть пустым")
            .ValidateOnStart();

        services.AddSingleton<IImportService, ImportService>();
        services.AddHostedService<ImportSchedulerWorker>();
        return services;
    }
}
