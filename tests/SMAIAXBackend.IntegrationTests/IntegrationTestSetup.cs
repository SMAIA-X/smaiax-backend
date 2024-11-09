using DotNet.Testcontainers.Builders;

using Microsoft.Extensions.DependencyInjection;

using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;
using SMAIAXBackend.Infrastructure.Repositories;

using Testcontainers.PostgreSql;

namespace SMAIAXBackend.IntegrationTests;

[SetUpFixture]
internal static class IntegrationTestSetup
{
    private static PostgreSqlContainer _postgresContainer = null!;
    private static WebAppFactory _webAppFactory = null!;
    public static ApplicationDbContext ApplicationDbContext { get; private set; } = null!;
    public static TenantDbContext TenantDbContext { get; private set; } = null!;
    public static ISmartMeterRepository SmartMeterRepository { get; private set; } = null!;
    public static IPolicyRepository PolicyRepository { get; private set; } = null!;
    public static IPolicyRequestRepository PolicyRequestRepository { get; private set; } = null!;
    public static IUserRepository UserRepository { get; private set; } = null!;
    public static ITenantRepository TenantRepository { get; private set; } = null!;
    public static HttpClient HttpClient { get; private set; } = null!;
    public static string AccessToken { get; private set; } = null!;

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        const int postgresPort = 5432;
        const string superUserName = "user";
        const string superUserPassword = "password";
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-bullseye")
            .WithUsername(superUserName)
            .WithPassword(superUserPassword)
            .WithDatabase("smaiax-db")
            .WithPortBinding(postgresPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(postgresPort))
            .Build();

        await _postgresContainer.StartAsync();

        var postgresMappedPublicPort = _postgresContainer.GetMappedPublicPort(postgresPort);
        _webAppFactory = new WebAppFactory(postgresMappedPublicPort);

        HttpClient = _webAppFactory.CreateClient();

        ApplicationDbContext = _webAppFactory.Services.GetRequiredService<ApplicationDbContext>();
        SmartMeterRepository = _webAppFactory.Services.GetRequiredService<ISmartMeterRepository>();
        var tenantDbContextFactory = _webAppFactory.Services.GetRequiredService<ITenantDbContextFactory>();
        TenantDbContext = tenantDbContextFactory.CreateDbContext("tenant_1_db", superUserName, superUserPassword);
        TenantRepository = _webAppFactory.Services.GetRequiredService<ITenantRepository>();
        UserRepository = _webAppFactory.Services.GetRequiredService<IUserRepository>();

        // Repositories that are using the TenantDatabase need to be instantiated because
        // They are injected with a connection string that is created based on the http request
        SmartMeterRepository = new SmartMeterRepository(TenantDbContext);
        PolicyRepository = new PolicyRepository(TenantDbContext);
        PolicyRequestRepository = new PolicyRequestRepository(TenantDbContext);

        var tokenRepository = _webAppFactory.Services.GetRequiredService<ITokenRepository>();
        AccessToken = await tokenRepository.GenerateAccessTokenAsync($"{Guid.NewGuid()}-{Guid.NewGuid()}",
            "3c07065a-b964-44a9-9cdf-fbd49d755ea7", "john.doe@example.com");
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
        HttpClient.Dispose();
        await _webAppFactory.DisposeAsync();
    }
}