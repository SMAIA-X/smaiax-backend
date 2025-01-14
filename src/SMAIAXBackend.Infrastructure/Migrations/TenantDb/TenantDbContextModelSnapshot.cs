﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SMAIAXBackend.Infrastructure.DbContexts;

#nullable disable

namespace SMAIAXBackend.Infrastructure.Migrations.TenantDb
{
    [DbContext(typeof(TenantDbContext))]
    partial class TenantDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.Contract", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt");

                    b.Property<Guid>("PolicyId")
                        .HasColumnType("uuid")
                        .HasColumnName("policyId");

                    b.HasKey("Id")
                        .HasName("pK_Contract");

                    b.ToTable("Contract", "domain");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.Measurements.Measurement", b =>
                {
                    b.Property<double>("CurrentPhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase1");

                    b.Property<double>("CurrentPhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase2");

                    b.Property<double>("CurrentPhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase3");

                    b.Property<double>("NegativeActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActiveEnergyTotal");

                    b.Property<double>("NegativeActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActivePower");

                    b.Property<double>("PositiveActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActiveEnergyTotal");

                    b.Property<double>("PositiveActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActivePower");

                    b.Property<double>("ReactiveEnergyQuadrant1Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant1Total");

                    b.Property<double>("ReactiveEnergyQuadrant3Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant3Total");

                    b.Property<Guid>("SmartMeterId")
                        .HasColumnType("uuid")
                        .HasColumnName("smartMeterId");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<double>("TotalPower")
                        .HasColumnType("double precision")
                        .HasColumnName("totalPower");

                    b.Property<string>("Uptime")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uptime");

                    b.Property<double>("VoltagePhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase1");

                    b.Property<double>("VoltagePhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase2");

                    b.Property<double>("VoltagePhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase3");

                    b.HasIndex("SmartMeterId")
                        .HasDatabaseName("iX_Measurement_smartMeterId");

                    b.HasIndex("Timestamp")
                        .IsDescending()
                        .HasDatabaseName("iX_Measurement_timestamp");

                    b.ToTable("Measurement", "domain");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.Measurements.MeasurementPerDay", b =>
                {
                    b.Property<double>("CurrentPhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase1");

                    b.Property<double>("CurrentPhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase2");

                    b.Property<double>("CurrentPhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase3");

                    b.Property<double>("NegativeActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActiveEnergyTotal");

                    b.Property<double>("NegativeActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActivePower");

                    b.Property<double>("PositiveActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActiveEnergyTotal");

                    b.Property<double>("PositiveActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActivePower");

                    b.Property<double>("ReactiveEnergyQuadrant1Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant1Total");

                    b.Property<double>("ReactiveEnergyQuadrant3Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant3Total");

                    b.Property<Guid>("SmartMeterId")
                        .HasColumnType("uuid")
                        .HasColumnName("smartMeterId");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<double>("TotalPower")
                        .HasColumnType("double precision")
                        .HasColumnName("totalPower");

                    b.Property<string>("Uptime")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uptime");

                    b.Property<double>("VoltagePhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase1");

                    b.Property<double>("VoltagePhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase2");

                    b.Property<double>("VoltagePhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase3");

                    b.ToTable((string)null);

                    b.ToView("MeasurementPerDay", "domain");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.Measurements.MeasurementPerHour", b =>
                {
                    b.Property<double>("CurrentPhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase1");

                    b.Property<double>("CurrentPhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase2");

                    b.Property<double>("CurrentPhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase3");

                    b.Property<double>("NegativeActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActiveEnergyTotal");

                    b.Property<double>("NegativeActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActivePower");

                    b.Property<double>("PositiveActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActiveEnergyTotal");

                    b.Property<double>("PositiveActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActivePower");

                    b.Property<double>("ReactiveEnergyQuadrant1Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant1Total");

                    b.Property<double>("ReactiveEnergyQuadrant3Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant3Total");

                    b.Property<Guid>("SmartMeterId")
                        .HasColumnType("uuid")
                        .HasColumnName("smartMeterId");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<double>("TotalPower")
                        .HasColumnType("double precision")
                        .HasColumnName("totalPower");

                    b.Property<string>("Uptime")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uptime");

                    b.Property<double>("VoltagePhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase1");

                    b.Property<double>("VoltagePhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase2");

                    b.Property<double>("VoltagePhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase3");

                    b.ToTable((string)null);

                    b.ToView("MeasurementPerHour", "domain");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.Measurements.MeasurementPerMinute", b =>
                {
                    b.Property<double>("CurrentPhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase1");

                    b.Property<double>("CurrentPhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase2");

                    b.Property<double>("CurrentPhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase3");

                    b.Property<double>("NegativeActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActiveEnergyTotal");

                    b.Property<double>("NegativeActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActivePower");

                    b.Property<double>("PositiveActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActiveEnergyTotal");

                    b.Property<double>("PositiveActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActivePower");

                    b.Property<double>("ReactiveEnergyQuadrant1Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant1Total");

                    b.Property<double>("ReactiveEnergyQuadrant3Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant3Total");

                    b.Property<Guid>("SmartMeterId")
                        .HasColumnType("uuid")
                        .HasColumnName("smartMeterId");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<double>("TotalPower")
                        .HasColumnType("double precision")
                        .HasColumnName("totalPower");

                    b.Property<string>("Uptime")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uptime");

                    b.Property<double>("VoltagePhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase1");

                    b.Property<double>("VoltagePhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase2");

                    b.Property<double>("VoltagePhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase3");

                    b.ToTable((string)null);

                    b.ToView("MeasurementPerMinute", "domain");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.Measurements.MeasurementPerQuarterHour", b =>
                {
                    b.Property<double>("CurrentPhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase1");

                    b.Property<double>("CurrentPhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase2");

                    b.Property<double>("CurrentPhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase3");

                    b.Property<double>("NegativeActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActiveEnergyTotal");

                    b.Property<double>("NegativeActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActivePower");

                    b.Property<double>("PositiveActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActiveEnergyTotal");

                    b.Property<double>("PositiveActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActivePower");

                    b.Property<double>("ReactiveEnergyQuadrant1Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant1Total");

                    b.Property<double>("ReactiveEnergyQuadrant3Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant3Total");

                    b.Property<Guid>("SmartMeterId")
                        .HasColumnType("uuid")
                        .HasColumnName("smartMeterId");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<double>("TotalPower")
                        .HasColumnType("double precision")
                        .HasColumnName("totalPower");

                    b.Property<string>("Uptime")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uptime");

                    b.Property<double>("VoltagePhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase1");

                    b.Property<double>("VoltagePhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase2");

                    b.Property<double>("VoltagePhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase3");

                    b.ToTable((string)null);

                    b.ToView("MeasurementPerQuarterHour", "domain");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.Measurements.MeasurementPerWeek", b =>
                {
                    b.Property<double>("CurrentPhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase1");

                    b.Property<double>("CurrentPhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase2");

                    b.Property<double>("CurrentPhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("currentPhase3");

                    b.Property<double>("NegativeActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActiveEnergyTotal");

                    b.Property<double>("NegativeActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("negativeActivePower");

                    b.Property<double>("PositiveActiveEnergyTotal")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActiveEnergyTotal");

                    b.Property<double>("PositiveActivePower")
                        .HasColumnType("double precision")
                        .HasColumnName("positiveActivePower");

                    b.Property<double>("ReactiveEnergyQuadrant1Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant1Total");

                    b.Property<double>("ReactiveEnergyQuadrant3Total")
                        .HasColumnType("double precision")
                        .HasColumnName("reactiveEnergyQuadrant3Total");

                    b.Property<Guid>("SmartMeterId")
                        .HasColumnType("uuid")
                        .HasColumnName("smartMeterId");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<double>("TotalPower")
                        .HasColumnType("double precision")
                        .HasColumnName("totalPower");

                    b.Property<string>("Uptime")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uptime");

                    b.Property<double>("VoltagePhase1")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase1");

                    b.Property<double>("VoltagePhase2")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase2");

                    b.Property<double>("VoltagePhase3")
                        .HasColumnType("double precision")
                        .HasColumnName("voltagePhase3");

                    b.ToTable((string)null);

                    b.ToView("MeasurementPerWeek", "domain");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.Metadata", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int?>("HouseholdSize")
                        .HasColumnType("integer")
                        .HasColumnName("householdSize");

                    b.Property<Guid>("SmartMeterId")
                        .HasColumnType("uuid")
                        .HasColumnName("smartMeterId");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("validFrom");

                    b.HasKey("Id")
                        .HasName("pK_Metadata");

                    b.HasIndex("SmartMeterId")
                        .HasDatabaseName("iX_Metadata_smartMeterId");

                    b.HasIndex("ValidFrom")
                        .IsUnique()
                        .HasDatabaseName("iX_Metadata_validFrom");

                    b.ToTable("Metadata", "domain");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.Policy", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("LocationResolution")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("locationResolution");

                    b.Property<string>("MeasurementResolution")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("measurementResolution");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("price");

                    b.Property<Guid>("SmartMeterId")
                        .HasColumnType("uuid")
                        .HasColumnName("smartMeterId");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("state");

                    b.HasKey("Id")
                        .HasName("pK_Policy");

                    b.ToTable("Policy", "domain");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.SmartMeter", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("ConnectorSerialNumber")
                        .HasColumnType("uuid")
                        .HasColumnName("connectorSerialNumber");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("PublicKey")
                        .HasColumnType("text")
                        .HasColumnName("publicKey");

                    b.HasKey("Id")
                        .HasName("pK_SmartMeter");

                    b.ToTable("SmartMeter", "domain");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.Metadata", b =>
                {
                    b.HasOne("SMAIAXBackend.Domain.Model.Entities.SmartMeter", "SmartMeter")
                        .WithMany("Metadata")
                        .HasForeignKey("SmartMeterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fK_Metadata_SmartMeter_smartMeterId");

                    b.OwnsOne("SMAIAXBackend.Domain.Model.ValueObjects.Location", "Location", b1 =>
                        {
                            b1.Property<Guid>("MetadataId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("City")
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)")
                                .HasColumnName("city");

                            b1.Property<string>("Continent")
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)")
                                .HasColumnName("continent");

                            b1.Property<string>("Country")
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)")
                                .HasColumnName("country");

                            b1.Property<string>("State")
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)")
                                .HasColumnName("state");

                            b1.Property<string>("StreetName")
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)")
                                .HasColumnName("streetName");

                            b1.HasKey("MetadataId");

                            b1.ToTable("Metadata", "domain");

                            b1.WithOwner()
                                .HasForeignKey("MetadataId")
                                .HasConstraintName("fK_Metadata_Metadata_id");
                        });

                    b.Navigation("Location");

                    b.Navigation("SmartMeter");
                });

            modelBuilder.Entity("SMAIAXBackend.Domain.Model.Entities.SmartMeter", b =>
                {
                    b.Navigation("Metadata");
                });
#pragma warning restore 612, 618
        }
    }
}
