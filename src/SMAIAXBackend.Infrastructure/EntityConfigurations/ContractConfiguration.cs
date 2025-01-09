using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Infrastructure.EntityConfigurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contract", "domain");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(
                v => v.Id,
                v => new ContractId(v))
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.PolicyId)
            .HasConversion(
                v => v.Id,
                v => new PolicyId(v))
            .IsRequired();
    }
}