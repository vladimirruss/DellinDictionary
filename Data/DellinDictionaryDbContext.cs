using System.Reflection;
using DellinDictionary.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DellinDictionary.Data;

public class DellinDictionaryDbContext(DbContextOptions<DellinDictionaryDbContext> options)
    : DbContext(options)
{
    public DbSet<Office> Offices => Set<Office>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
