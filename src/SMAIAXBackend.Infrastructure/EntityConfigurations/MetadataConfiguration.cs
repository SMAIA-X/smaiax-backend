using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class MetadataConfiguration : IEntityTypeConfiguration<Metadata>
{
    public void Configure(EntityTypeBuilder<Metadata> builder)
    {
        builder.ToTable("Metadata", "domain");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasConversion(
                v => v.Id,
                v => new MetadataId(v))
            .IsRequired();

        builder.Property(m => m.ValidFrom)
            .IsRequired();

        builder.OwnsOne(m => m.Location, location =>
        {
            location.Property(l => l.StreetName).HasColumnName("StreetName");
            location.Property(l => l.City).HasColumnName("City");
            location.Property(l => l.State).HasColumnName("State");
            location.Property(l => l.Country).HasColumnName("Country");
            location.Property(l => l.Continent).HasColumnName("Continent").HasConversion<string>();
        });

        builder.Property(m => m.HouseholdSize)
            .IsRequired();

        builder.HasOne(md => md.SmartMeter)
            .WithMany(sm => sm.Metadata)
            .HasForeignKey(md => md.SmartMeterId)
            .IsRequired();
    }
}