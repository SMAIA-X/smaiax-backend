using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Infrastructure.EntityConfigurations;

namespace SMAIAXBackend.Infrastructure.DbContexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<User> DomainUsers { get; init; }
    public DbSet<RefreshToken> RefreshTokens { get; init; }
    public DbSet<Tenant> Tenants { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new DomainUserConfiguration());
        builder.ApplyConfiguration(new RefreshTokenConfiguration());
        builder.ApplyConfiguration(new TenantConfiguration());

        // Place Identity tables in the "auth" schema
        builder.Entity<IdentityUser>(entity => entity.ToTable(name: "AspNetUsers", schema: "auth"));
        builder.Entity<IdentityRole>(entity => entity.ToTable(name: "AspNetRoles", schema: "auth"));
        builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("AspNetUserRoles", schema: "auth"));
        builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("AspNetUserClaims", schema: "auth"));
        builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("AspNetUserLogins", schema: "auth"));
        builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("AspNetRoleClaims", schema: "auth"));
        builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("AspNetUserTokens", schema: "auth"));
    }

    public async Task SeedTestData()
    {
        var hasher = new PasswordHasher<IdentityUser>();

        var userId = new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7"));
        var userName = configuration.GetValue<string>("TestUser:Username");
        var email = configuration.GetValue<string>("TestUser:Email");
        var password = configuration.GetValue<string>("TestUser:Password");
        var tenantDatabase = configuration.GetValue<string>("TestUser:Database");

        var testUser = new IdentityUser
        {
            Id = userId.Id.ToString(),
            UserName = userName,
            NormalizedUserName = userName!.ToUpper(),
            Email = email,
            NormalizedEmail = email!.ToUpper(),
        };
        var passwordHash = hasher.HashPassword(testUser, password!);
        testUser.PasswordHash = passwordHash;

        var tenant = Tenant.Create(new TenantId(Guid.NewGuid()), "tenant_1_role", tenantDatabase!);
        var domainUser = User.Create(userId, new Name("John", "Doe"), userName, email, tenant.Id);

        await Users.AddAsync(testUser);
        await Tenants.AddAsync(tenant);
        await DomainUsers.AddAsync(domainUser);
        await SaveChangesAsync();
    }
}