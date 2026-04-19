using DellinDictionary.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DellinDictionary.Data.Configurations;

public class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        builder.ToTable("offices");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Code).HasMaxLength(64);
        builder.Property(o => o.Uuid).HasMaxLength(64);
        builder.Property(o => o.CountryCode).HasMaxLength(8).IsRequired();
        builder.Property(o => o.AddressRegion).HasMaxLength(256);
        builder.Property(o => o.AddressCity).HasMaxLength(256);
        builder.Property(o => o.AddressStreet).HasMaxLength(256);
        builder.Property(o => o.AddressHouseNumber).HasMaxLength(64);
        builder.Property(o => o.WorkTime).HasMaxLength(512).IsRequired();

        builder.Property(o => o.Type).HasConversion<string>().HasMaxLength(16);

        builder.OwnsOne(o => o.Coordinates, coord =>
        {
            coord.Property(c => c.Latitude).HasColumnName("latitude");
            coord.Property(c => c.Longitude).HasColumnName("longitude");
        });

        builder.HasOne(o => o.Phones)
            .WithOne(p => p.Office)
            .HasForeignKey<Phone>(p => p.OfficeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(o => o.CityCode);
        builder.HasIndex(o => o.AddressCity);
        builder.HasIndex(o => new { o.AddressCity, o.AddressRegion });
    }
}
