using System.Text.Json.Serialization;
using DellinDictionary.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default не задан");

builder.Services.AddDatabase(connectionString);
builder.Services.AddImportScheduler(builder.Configuration);
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

await app.ApplyMigrationsAsync();

app.MapControllers();

app.Run();
