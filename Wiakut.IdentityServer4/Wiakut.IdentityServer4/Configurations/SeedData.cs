using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wiakut.IdentityServer4.Context;

namespace Wiakut.IdentityServer4.Configurations;

public static class SeedData
{
    public static async Task EnsureSeedDataAsync(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        
        services.AddDbContext<AspNetIdentityDbContext>(options =>
            options.UseSqlServer(connectionString));

        services
            .AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AspNetIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddOperationalDbContext(options =>
        {
            options.ConfigureDbContext = db =>
                db.UseSqlServer(connectionString, action =>
                    action.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
        });
        
        services.AddConfigurationDbContext(options =>
        {
            options.ConfigureDbContext = db =>
                db.UseSqlServer(connectionString, action =>
                    action.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
        });

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        await scope.ServiceProvider.GetService<PersistedGrantDbContext>()?.Database.MigrateAsync()!;
        
        var configurationContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
        await configurationContext?.Database.MigrateAsync()!;
        
        EnsureIdentityServerData(configurationContext);

        var aspNetIdentityContext = scope.ServiceProvider.GetService<AspNetIdentityDbContext>();
        await aspNetIdentityContext?.Database.MigrateAsync()!;
        
        await EnsureUsersAsync(scope);
    }

    private static void EnsureIdentityServerData(ConfigurationDbContext? context)
    {
        if(context is null)
            return;

        if (!context.Clients.Any())
        {
            foreach (var client in IdentityServerData.Clients.ToList())
            {
                context.Clients.Add(client.ToEntity());
            }

            context.SaveChanges();
        }
        
        if (!context.IdentityResources.Any())
        {
            foreach (var identityResource in IdentityServerData.IdentityResources.ToList())
            {
                context.IdentityResources.Add(identityResource.ToEntity());
            }

            context.SaveChanges();
        }
        
        if (!context.ApiScopes.Any())
        {
            foreach (var apiScope in IdentityServerData.ApiScopes.ToList())
            {
                context.ApiScopes.Add(apiScope.ToEntity());
            }

            context.SaveChanges();
        }
        
        if (!context.ApiResources.Any())
        {
            foreach (var apiResource in IdentityServerData.ApiResources.ToList())
            {
                context.ApiResources.Add(apiResource.ToEntity());
            }

            context.SaveChanges();
        }
    }

    private static async Task EnsureUsersAsync(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();

        var wiakutUser = await userManager.FindByNameAsync("WiakutUser");
        if (wiakutUser == null)
        {
            wiakutUser = new IdentityUser<Guid>
            {
                UserName = "WiakutUser",
                Email = "wiakut@email.com",
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(wiakutUser, "Wiakut@123");
            if (!createResult.Succeeded)
            {
                throw new Exception(createResult.Errors.FirstOrDefault()?.Description);
            }

            var claimsResult = await userManager.AddClaimsAsync(
                wiakutUser, new Claim[]
                {
                    new (JwtClaimTypes.Name, "Wiakut UserName"),
                    new (JwtClaimTypes.GivenName, "Wiakut"),
                    new (JwtClaimTypes.FamilyName, "UserName")
                });

            if (!claimsResult.Succeeded)
            {
                throw new Exception(claimsResult.Errors.FirstOrDefault()?.Description);
            }
        }
    }
}