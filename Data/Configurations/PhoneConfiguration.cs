using DellinDictionary.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DellinDictionary.Data.Configurations;

public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
{
    public void Configure(EntityTypeBuilder<Phone> builder)
    {
        builder.ToTable("phones");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PhoneNumber).HasMaxLength(64).IsRequired();
        builder.Property(p => p.Additional).HasMaxLength(64);

        builder.HasIndex(p => p.OfficeId).IsUnique();
    }
}
