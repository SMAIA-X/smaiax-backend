using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Npgsql;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.IntegrationTests;

public class TestBase
{
    protected readonly HttpClient _httpClient = IntegrationTestSetup.HttpClient;
    protected readonly ApplicationDbContext _applicationDbContext = IntegrationTestSetup.ApplicationDbContext;
    protected readonly TenantDbContext _tenant1DbContext = IntegrationTestSetup.Tenant1DbContext;
    protected readonly TenantDbContext _tenant2DbContext = IntegrationTestSetup.Tenant2DbContext;
    protected readonly ISmartMeterRepository _smartMeterRepository = IntegrationTestSetup.SmartMeterRepository;
    protected readonly IMeasurementRepository _measurementRepository = IntegrationTestSetup.MeasurementRepository;
    protected readonly IPolicyRepository _policyRepository = IntegrationTestSetup.PolicyRepository;
    protected readonly IUserRepository _userRepository = IntegrationTestSetup.UserRepository;
    protected readonly string _accessToken = IntegrationTestSetup.AccessToken;

    [SetUp]
    public async Task Setup()
    {
        await IntegrationTestSetup.ApplicationDbContext.Database.EnsureCreatedAsync();
        await IntegrationTestSetup.TenantRepository.CreateDatabaseForTenantAsync("tenant_1_db");
        await IntegrationTestSetup.TenantRepository.CreateDatabaseForTenantAsync("tenant_2_db");
        await InsertTestData();
        IntegrationTestSetup.ApplicationDbContext.ChangeTracker.Clear();
        IntegrationTestSetup.Tenant1DbContext.ChangeTracker.Clear();
        IntegrationTestSetup.Tenant2DbContext.ChangeTracker.Clear();
        await IntegrationTestSetup.VaultRepository.CreateDatabaseRoleAsync("tenant_1_role", "tenant_1_db");
        await IntegrationTestSetup.VaultRepository.CreateDatabaseRoleAsync("tenant_2_role", "tenant_2_db");
    }

    [TearDown]
    public async Task TearDown()
    {
        IntegrationTestSetup.ApplicationDbContext.ChangeTracker.Clear();
        IntegrationTestSetup.Tenant1DbContext.ChangeTracker.Clear();
        IntegrationTestSetup.Tenant2DbContext.ChangeTracker.Clear();
        await IntegrationTestSetup.Tenant1DbContext.Database.EnsureDeletedAsync();
        await IntegrationTestSetup.Tenant2DbContext.Database.EnsureDeletedAsync();
        await IntegrationTestSetup.ApplicationDbContext.Database.EnsureDeletedAsync();
    }

    private async Task InsertTestData()
    {
        var hasher = new PasswordHasher<IdentityUser>();

        var johnDoeUserId = new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7"));
        const string johnDoeUserName = "johndoe";
        const string johnDoeEmail = "john.doe@example.com";
        const string johnDoePassword = "P@ssw0rd";
        var johnDoeTestUser = new IdentityUser
        {
            Id = johnDoeUserId.Id.ToString(),
            UserName = johnDoeUserName,
            NormalizedUserName = johnDoeUserName.ToUpper(),
            Email = johnDoeEmail,
            NormalizedEmail = johnDoeEmail.ToUpper(),
        };
        var johnDoePasswordHash = hasher.HashPassword(johnDoeTestUser, johnDoePassword);
        johnDoeTestUser.PasswordHash = johnDoePasswordHash;

        var johnDoeTenantId = new TenantId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"));
        var johnDoeTenant = Tenant.Create(johnDoeTenantId, "tenant_1_role", "tenant_1_db");
        var johnDoeDomainUser = User.Create(johnDoeUserId, new Name("John", "Doe"), johnDoeUserName, johnDoeEmail,
            johnDoeTenantId);

        var janeDoeUserId = new UserId(Guid.Parse("4d07065a-b964-44a9-9cdf-fbd49d755ea8"));
        const string janeDoeUserName = "janedoe";
        const string janeDoeEmail = "jane.doe@example.com";
        const string janeDoePassword = "P@ssw0rd";
        var janeDoeTestUser = new IdentityUser
        {
            Id = janeDoeUserId.Id.ToString(),
            UserName = janeDoeUserName,
            NormalizedUserName = janeDoeUserName.ToUpper(),
            Email = janeDoeEmail,
            NormalizedEmail = janeDoeEmail.ToUpper(),
        };
        var janeDoePasswordHash = hasher.HashPassword(janeDoeTestUser, janeDoePassword);
        janeDoeTestUser.PasswordHash = janeDoePasswordHash;

        var janeDoeTenantId = new TenantId(Guid.Parse("e4c70232-6715-4c15-966f-bf4bcef46d40"));
        var janeDoeTenant = Tenant.Create(janeDoeTenantId, "tenant_2_role", "tenant_2_db");
        var janeDoeDomainUser = User.Create(janeDoeUserId, new Name("Jane", "Doe"), janeDoeUserName, janeDoeEmail,
            janeDoeTenantId);

        // Valid refresh token
        const string jwtId = "19f77b2e-e485-4031-8506-62f6d3b69e4d";
        const string token1 = "4dffb63c-581d-4588-8b4b-4b075f17d015-abcb30f4-5f32-4fbb-80c4-99cea98273ca";
        var expirationDate1 = DateTime.UtcNow.AddDays(100);
        var refreshToken1 = RefreshToken.Create(
            new RefreshTokenId(Guid.Parse("21938ead-d43f-4f16-a055-c3b5613cd599")),
            johnDoeUserId,
            jwtId,
            token1,
            true,
            expirationDate1
        );

        // Invalid refresh token
        const string token2 = "266cbbdb-edcd-48a6-aa63-f837b05a2551-3b01aaa3-304a-434b-bc7d-fd9a6305550b";
        var refreshToken2 = RefreshToken.Create(
            new RefreshTokenId(Guid.Parse("ad758462-20de-41fc-91d4-0569466224fc")),
            johnDoeUserId,
            jwtId,
            token2,
            false,
            expirationDate1
        );

        // Expired refresh token
        const string token3 = "01318f82-8307-480d-bbb6-f3be92ba7480-b903a69c-7d76-4ce1-8dce-d325712bf240";
        var expirationDate2 = DateTime.UtcNow.AddDays(-10);
        var refreshToken3 = RefreshToken.Create(
            new RefreshTokenId(Guid.Parse("9674b31b-eee3-47c1-be45-f49c4c3004f3")),
            johnDoeUserId,
            jwtId,
            token3,
            true,
            expirationDate2
        );

        // Refresh token for an access token where the user can't be found
        const string token4 = "90502660-ebbb-405a-9c93-0bd4e9c2ba41-714b9f49-e7b9-4090-8e84-61fdbe8e9f6e";
        const string jwtId2 = "87039e2f-867e-409d-bb60-e8dabc84f52d";
        var invalidUserId = new UserId(Guid.Parse("1e09ca29-2910-4f54-8002-2d9e063090c6"));
        var refreshToken4 = RefreshToken.Create(
            new RefreshTokenId(Guid.Parse("03cb12a5-26f4-4bcd-b6fb-ff5d82f9bf10")),
            invalidUserId,
            jwtId2,
            token4,
            true,
            expirationDate1
        );

        // John Doe
        var smartMeter2Id = new SmartMeterId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"));
        var smartMeter2Metadata = Metadata.Create(new MetadataId(Guid.Parse("1c8c8313-6fc4-4ebd-9ca8-8a1267441e06")),
            DateTime.UtcNow, new Location("Some Streetname", "Some city", "Some state", "Some county", Continent.Asia),
            4, smartMeter2Id);
        var smartMeter1 = SmartMeter.Create(new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd")),
            "Smart Meter 1", [], new ConnectorSerialNumber(Guid.NewGuid()),
            "MIICCgKCAgEAncWYOkd7VTOFoE4QJuvdEAP1x+PJVM9bRf+Bs9t1V3NDn5tNUplP0Y6fbamWTAh3Irji5KwxNCSrDVtX6IJ7qDRRN6TG3kbBLngyWQsajO7laLYshFE86jseuEGx3hu9bbN3Q1gLsVb2mkJukw6v0LugiSK++3wFisfVTOAe59XxE0geMTyghnc2jak/LEI5nNoe85yinVAzCQiHJqxqjA93IWwKT7MMOUoVHXOnPd84TeXIuPKNHhfG5J/K545z7cyzLfEfyCvCs6cxsjpFNilrWqxmh4J9ukcooVv7p3s7DSJNWsEbaW6XC4Q+wvy6aHmIVE7llgyV216+qWr8EMCMcHDTnaXr1/PcLFfqOelCgqAU2aIFIZvrAl2GZruFHso/VbryMq9iPQQK5nfJJNisUCQRtUIehTajfGMfBiI62lgqV8Qa/J2pksYppLzX9vQlEa3IsMOCkIwkK/sHHkOx+dfyVoASYnhMDHfQ35aGpCZpaM9XdVfcE0Aip5plpgxDMefg70Ur1TfGJFqD5ix04ehIewg2oh6yu/nU5jAMVm9CsKqUcmpmQkdp2pEb4s6t4A2aIgPMpZpzJmp4WDEsr0v+Bo+kFRDNnWK9dfxav6duE3fLL/IJiX6YfmbRsC0mC+7Mmptg1reeI7xgw4eWfDQluKx7z+2uHQVcgqECAwEAAQ==");
        var smartMeter2 = SmartMeter.Create(smartMeter2Id, "Smart Meter 2", [smartMeter2Metadata],
            new ConnectorSerialNumber(Guid.NewGuid()),
            "MIICCgKCAgEAncWYOkd7VTOFoE4QJuvdEAP1x+PJVM9bRf+Bs9t1V3NDn5tNUplP0Y6fbamWTAh3Irji5KwxNCSrDVtX6IJ7qDRRN6TG3kbBLngyWQsajO7laLYshFE86jseuEGx3hu9bbN3Q1gLsVb2mkJukw6v0LugiSK++3wFisfVTOAe59XxE0geMTyghnc2jak/LEI5nNoe85yinVAzCQiHJqxqjA93IWwKT7MMOUoVHXOnPd84TeXIuPKNHhfG5J/K545z7cyzLfEfyCvCs6cxsjpFNilrWqxmh4J9ukcooVv7p3s7DSJNWsEbaW6XC4Q+wvy6aHmIVE7llgyV216+qWr8EMCMcHDTnaXr1/PcLFfqOelCgqAU2aIFIZvrAl2GZruFHso/VbryMq9iPQQK5nfJJNisUCQRtUIehTajfGMfBiI62lgqV8Qa/J2pksYppLzX9vQlEa3IsMOCkIwkK/sHHkOx+dfyVoASYnhMDHfQ35aGpCZpaM9XdVfcE0Aip5plpgxDMefg70Ur1TfGJFqD5ix04ehIewg2oh6yu/nU5jAMVm9CsKqUcmpmQkdp2pEb4s6t4A2aIgPMpZpzJmp4WDEsr0v+Bo+kFRDNnWK9dfxav6duE3fLL/IJiX6YfmbRsC0mC+7Mmptg1reeI7xgw4eWfDQluKx7z+2uHQVcgqECAwEAAQ==");
        var policyRequest = PolicyRequest.Create(
            new PolicyRequestId(Guid.Parse("58af578c-9975-4633-8dfe-ff8b70b83661")),
            false, new PolicyFilter(MeasurementResolution.Hour, 1, 10, [],
                LocationResolution.State, 500));

        // Jane Doe
        var smartMeter3Id = new SmartMeterId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d49"));
        var smartMeter3Metadata = Metadata.Create(new MetadataId(Guid.Parse("1c8c8313-6fc4-4ebd-9ca8-8a1267441e07")),
            DateTime.UtcNow, new Location("Some Streetname", "Some city", "Some state", "Some county", Continent.Asia),
            4, smartMeter3Id);
        var smartMeter3 = SmartMeter.Create(smartMeter3Id,
            "Smart Meter 3", [smartMeter3Metadata], new ConnectorSerialNumber(Guid.NewGuid()),
            "MIICCgKCAgEAncWYOkd7VTOFoE4QJuvdEAP1x+PJVM9bRf+Bs9t1V3NDn5tNUplP0Y6fbamWTAh3Irji5KwxNCSrDVtX6IJ7qDRRN6TG3kbBLngyWQsajO7laLYshFE86jseuEGx3hu9bbN3Q1gLsVb2mkJukw6v0LugiSK++3wFisfVTOAe59XxE0geMTyghnc2jak/LEI5nNoe85yinVAzCQiHJqxqjA93IWwKT7MMOUoVHXOnPd84TeXIuPKNHhfG5J/K545z7cyzLfEfyCvCs6cxsjpFNilrWqxmh4J9ukcooVv7p3s7DSJNWsEbaW6XC4Q+wvy6aHmIVE7llgyV216+qWr8EMCMcHDTnaXr1/PcLFfqOelCgqAU2aIFIZvrAl2GZruFHso/VbryMq9iPQQK5nfJJNisUCQRtUIehTajfGMfBiI62lgqV8Qa/J2pksYppLzX9vQlEa3IsMOCkIwkK/sHHkOx+dfyVoASYnhMDHfQ35aGpCZpaM9XdVfcE0Aip5plpgxDMefg70Ur1TfGJFqD5ix04ehIewg2oh6yu/nU5jAMVm9CsKqUcmpmQkdp2pEb4s6t4A2aIgPMpZpzJmp4WDEsr0v+Bo+kFRDNnWK9dfxav6duE3fLL/IJiX6YfmbRsC0mC+7Mmptg1reeI7xgw4eWfDQluKx7z+2uHQVcgqECAwEAAQ==");
        var policy1 = Policy.Create(new PolicyId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39")), "policy1",
            MeasurementResolution.Hour, LocationResolution.Country, 500, smartMeter3Id);
        var policy2 = Policy.Create(new PolicyId(Guid.Parse("a4c70232-6715-4c15-966f-bf4bcef46d40")), "policy2",
            MeasurementResolution.Minute, LocationResolution.City, 1000, smartMeter3Id);

        var unassignedSmartMeterId = new SmartMeterId(Guid.Parse("1355836c-ba6c-4e23-b48a-72b77025bd6b"));
        var unassignedSmartMeterSerialNumber = new ConnectorSerialNumber(Guid.Parse("31c4fd82-5018-4bcd-bc0e-74d6b0a4e86d"));
        var unassignedSmartMeter = SmartMeter.Create(unassignedSmartMeterId, "Smart Meter 01", unassignedSmartMeterSerialNumber, "");

        await _applicationDbContext.Tenants.AddAsync(johnDoeTenant);
        await _applicationDbContext.Tenants.AddAsync(janeDoeTenant);
        await _applicationDbContext.Users.AddAsync(johnDoeTestUser);
        await _applicationDbContext.Users.AddAsync(janeDoeTestUser);
        await _applicationDbContext.DomainUsers.AddAsync(johnDoeDomainUser);
        await _applicationDbContext.DomainUsers.AddAsync(janeDoeDomainUser);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken1);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken2);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken3);
        await _applicationDbContext.RefreshTokens.AddAsync(refreshToken4);
        await _tenant1DbContext.SmartMeters.AddAsync(smartMeter1);
        await _tenant1DbContext.SmartMeters.AddAsync(smartMeter2);
        await _tenant1DbContext.SmartMeters.AddAsync(unassignedSmartMeter);
        await _tenant1DbContext.PolicyRequests.AddAsync(policyRequest);
        await _tenant2DbContext.SmartMeters.AddAsync(smartMeter3);
        await _tenant2DbContext.Policies.AddAsync(policy1);
        await _tenant2DbContext.Policies.AddAsync(policy2);

        await _applicationDbContext.SaveChangesAsync();
        await _tenant1DbContext.SaveChangesAsync();
        await _tenant2DbContext.SaveChangesAsync();

        // can't be inserted via "AddAsync".
        await _tenant1DbContext.Database.OpenConnectionAsync();
        var sql = @"
            INSERT INTO domain.""Measurement""(""positiveActivePower"", ""positiveActiveEnergyTotal"", ""negativeActivePower"", ""negativeActiveEnergyTotal"", ""reactiveEnergyQuadrant1Total"", ""reactiveEnergyQuadrant3Total"", ""totalPower"", ""currentPhase1"", ""voltagePhase1"", ""currentPhase2"", ""voltagePhase2"", ""currentPhase3"", ""voltagePhase3"", ""uptime"", ""timestamp"", ""smartMeterId"") 
            VALUES (@positiveActivePower, @positiveActiveEnergyTotal, @negativeActivePower, @negativeActiveEnergyTotal, @reactiveEnergyQuadrant1Total, @reactiveEnergyQuadrant3Total, @totalPower, @currentPhase1, @voltagePhase1, @currentPhase2, @voltagePhase2, @currentPhase3, @voltagePhase3, @uptime, @timestamp, @smartMeterId);
        ";
        await using var insertCommand = _tenant1DbContext.Database.GetDbConnection().CreateCommand();
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
        insertCommand.Parameters.Add(new NpgsqlParameter("@timestamp", DateTime.UtcNow.AddDays(-1)));
        insertCommand.Parameters.Add(new NpgsqlParameter("@smartMeterId", smartMeter1.Id.Id));

        await insertCommand.ExecuteNonQueryAsync();
        await _tenant1DbContext.Database.CloseConnectionAsync();
    }
}