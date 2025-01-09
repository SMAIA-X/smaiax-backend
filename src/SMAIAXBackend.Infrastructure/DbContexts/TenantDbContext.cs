using Microsoft.EntityFrameworkCore;

using Npgsql;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Entities.Measurements;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Infrastructure.EntityConfigurations;

namespace SMAIAXBackend.Infrastructure.DbContexts;

public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<SmartMeter> SmartMeters { get; init; }
    public DbSet<Measurement> Measurements { get; init; }
    public DbSet<MeasurementPerMinute> MeasurementsPerMinute { get; init; }
    public DbSet<MeasurementPerQuarterHour> MeasurementsPerQuarterHour { get; init; }
    public DbSet<MeasurementPerHour> MeasurementsPerHour { get; init; }
    public DbSet<MeasurementPerDay> MeasurementsPerDay { get; init; }
    public DbSet<MeasurementPerWeek> MeasurementsPerWeek { get; init; }
    public DbSet<Policy> Policies { get; init; }
    public DbSet<Contract> Contracts { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new MeasurementConfiguration());
        modelBuilder.ApplyConfiguration(new MeasurementPerMinuteConfiguration());
        modelBuilder.ApplyConfiguration(new MeasurementPerQuarterHourConfiguration());
        modelBuilder.ApplyConfiguration(new MeasurementPerHourConfiguration());
        modelBuilder.ApplyConfiguration(new MeasurementPerDayConfiguration());
        modelBuilder.ApplyConfiguration(new MeasurementPerWeekConfiguration());
        modelBuilder.ApplyConfiguration(new MetadataConfiguration());
        modelBuilder.ApplyConfiguration(new PolicyConfiguration());
        modelBuilder.ApplyConfiguration(new SmartMeterConfiguration());
        modelBuilder.ApplyConfiguration(new ContractConfiguration());
    }

    public async Task SeedTestData()
    {
        SmartMeterId smartMeter1Id = new(Guid.Parse("070dec95-56bb-4154-a2c4-c26faf9fff4d"));
        Metadata metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location("Hochschulstraße 1", "Dornbirn", "Vorarlberg", "Österreich", Continent.Oceania),
            4, smartMeter1Id);
        SmartMeter smartMeter1 = SmartMeter.Create(smartMeter1Id, "Smart Meter 1", [metadata]);
        SmartMeter smartMeter2 = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 2", []);

        var policy = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Hour,
            LocationResolution.None, 100, smartMeter1Id);
        var policy2 = Policy.Create(new PolicyId(Guid.NewGuid()), "policy2", MeasurementResolution.Day,
            LocationResolution.StreetName, 999, smartMeter1Id);
        var policy3 = Policy.Create(new PolicyId(Guid.NewGuid()), "policy3", MeasurementResolution.Raw,
            LocationResolution.Continent, 1999, smartMeter1Id);

        await Policies.AddAsync(policy);
        await Policies.AddAsync(policy2);
        await Policies.AddAsync(policy3);

        await SmartMeters.AddAsync(smartMeter1);
        await SmartMeters.AddAsync(smartMeter2);

        await SaveChangesAsync();

        // Can't be inserted via "AddAsync".
        await Database.OpenConnectionAsync();
        var sql = @"
            INSERT INTO domain.""Measurement""(""positiveActivePower"", ""positiveActiveEnergyTotal"", ""negativeActivePower"", ""negativeActiveEnergyTotal"", ""reactiveEnergyQuadrant1Total"", ""reactiveEnergyQuadrant3Total"", ""totalPower"", ""currentPhase1"", ""voltagePhase1"", ""currentPhase2"", ""voltagePhase2"", ""currentPhase3"", ""voltagePhase3"", ""uptime"", ""timestamp"", ""smartMeterId"") 
            VALUES (@positiveActivePower, @positiveActiveEnergyTotal, @negativeActivePower, @negativeActiveEnergyTotal, @reactiveEnergyQuadrant1Total, @reactiveEnergyQuadrant3Total, @totalPower, @currentPhase1, @voltagePhase1, @currentPhase2, @voltagePhase2, @currentPhase3, @voltagePhase3, @uptime, @timestamp, @smartMeterId);
        ";
        await using var insertCommand = Database.GetDbConnection().CreateCommand();
        insertCommand.CommandText = sql;

        insertCommand.Parameters.Add(new NpgsqlParameter("@positiveActivePower", 160));
        insertCommand.Parameters.Add(new NpgsqlParameter("@positiveActiveEnergyTotal", 1137778));
        insertCommand.Parameters.Add(new NpgsqlParameter("@negativeActivePower", 1));
        insertCommand.Parameters.Add(new NpgsqlParameter("@negativeActiveEnergyTotal", 1));
        insertCommand.Parameters.Add(new NpgsqlParameter("@reactiveEnergyQuadrant1Total", 3837));
        insertCommand.Parameters.Add(new NpgsqlParameter("@reactiveEnergyQuadrant3Total", 717727));
        insertCommand.Parameters.Add(new NpgsqlParameter("@totalPower", 160));
        insertCommand.Parameters.Add(new NpgsqlParameter("@currentPhase1", 1.03));
        insertCommand.Parameters.Add(new NpgsqlParameter("@voltagePhase1", 229.80));
        insertCommand.Parameters.Add(new NpgsqlParameter("@currentPhase2", 0.42));
        insertCommand.Parameters.Add(new NpgsqlParameter("@voltagePhase2", 229.00));
        insertCommand.Parameters.Add(new NpgsqlParameter("@currentPhase3", 0.17));
        insertCommand.Parameters.Add(new NpgsqlParameter("@voltagePhase3", 229.60));
        insertCommand.Parameters.Add(new NpgsqlParameter("@uptime", "0000:01:49:41"));
        insertCommand.Parameters.Add(new NpgsqlParameter("@timestamp", DateTime.UtcNow));
        insertCommand.Parameters.Add(new NpgsqlParameter("@smartMeterId", smartMeter1Id.Id));

        await insertCommand.ExecuteNonQueryAsync();
        await Database.CloseConnectionAsync();
    }
}