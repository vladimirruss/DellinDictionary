namespace DellinDictionary.Services;

public interface IImportService
{
    Task RunAsync(CancellationToken cancellationToken);
}
