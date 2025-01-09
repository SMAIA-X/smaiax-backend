using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using SMAIAXBackend.API.ApplicationConfigurations;
using SMAIAXBackend.API.Endpoints.Authentication;
using SMAIAXBackend.API.Endpoints.Contract;
using SMAIAXBackend.API.Endpoints.DeviceConfig;
using SMAIAXBackend.API.Endpoints.Measurement;
using SMAIAXBackend.API.Endpoints.Order;
using SMAIAXBackend.API.Endpoints.Policy;
using SMAIAXBackend.API.Endpoints.SmartMeter;
using SMAIAXBackend.API.Middlewares;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.Configurations;
using SMAIAXBackend.Infrastructure.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConfigurations(builder.Configuration);
builder.Services.AddRepositoryConfigurations();
builder.Services.AddServiceConfigurations();
builder.Services.AddIdentityConfigurations();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddExternalServiceConfigurations(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandlerMiddleware>();
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Add Swagger if in development environment
if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("DockerDevelopment"))
{
    // only used for api generation
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

    builder.Configuration.AddJsonFile("Properties/launchSettings.json", true, true);
    builder.Services.AddSwaggerConfigurations(builder.Configuration);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DockerDevelopment"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
    var tenantRepository = services.GetRequiredService<ITenantRepository>();
    var tenantDbContextFactory = services.GetRequiredService<ITenantDbContextFactory>();
    var vaultRepository = services.GetRequiredService<IVaultRepository>();

    await applicationDbContext.Database.EnsureDeletedAsync();
    await applicationDbContext.Database.EnsureCreatedAsync();
    await applicationDbContext.SeedTestData();

    // Create a database for the test user with test data for development
    var dbConfig = app.Configuration.GetSection("DatabaseConfiguration").Get<DatabaseConfiguration>();
    var testUserDatabase = app.Configuration.GetValue<string>("TestUser:Database");

    var tenantDbContext =
        tenantDbContextFactory.CreateDbContext("tenant_1_db", dbConfig!.SuperUsername, dbConfig.SuperUserPassword);
    await tenantDbContext.Database.EnsureDeletedAsync();
    await tenantRepository.CreateDatabaseForTenantAsync(testUserDatabase!);
    await tenantDbContext.SeedTestData();
    await vaultRepository.CreateDatabaseRoleAsync("tenant_1_role", "tenant_1_db");
}

app.UseAuthorization();
app.UseMiddleware<JwtClaimMiddleware>();
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapAuthenticationEndpoints()
    .MapSmartMeterEndpoints()
    .MapPolicyEndpoints()
    .MapDeviceConfigEndpoints()
    .MapOrderEndpoints()
    .MapMeasurementEndpoints()
    .MapContractEndpoints();

await app.RunAsync();

// For integration tests
[ExcludeFromCodeCoverage]
public abstract partial class Program
{
}