using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class SmartMeterConfiguration : IEntityTypeConfiguration<SmartMeter>
{
    public void Configure(EntityTypeBuilder<SmartMeter> builder)
    {
        builder.ToTable("SmartMeter", "domain");

        builder.HasKey(sm => sm.Id);
        builder.Property(sm => sm.Id)
            .HasConversion(
                v => v.Id,
                v => new SmartMeterId(v))
            .IsRequired();

        builder.Property(sm => sm.ConnectorSerialNumber)
            .HasConversion(
                v => v.SerialNumber,
                v => new ConnectorSerialNumber(v))
            .IsRequired();

        builder.Property(sm => sm.Name).IsRequired(false);

        builder.HasMany(sm => sm.Metadata)
            .WithOne(m => m.SmartMeter)
            .HasForeignKey(m => m.SmartMeterId).IsRequired();

        builder.Property(sm => sm.PublicKey).IsRequired(false);
    }
}