using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wiakut.IdentityServer4.Context;

namespace Wiakut.IdentityServer4.Configurations;

public static class IdentityServerPersistenceConfiguration
{
    public static async void ConfigureIdentityPersistence(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var assembly = typeof(IdentityServerPersistenceConfiguration).Assembly.GetName().FullName;
        var connectionString = configuration.GetConnectionString("IdentityDatabaseConnection");

        services.AddDbContext<AspNetIdentityDbContext>(options => 
            options.UseSqlServer(connectionString, action => action.MigrationsAssembly(assembly)));

        services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AspNetIdentityDbContext>();

        services.AddIdentityServer()
            .AddAspNetIdentity<IdentityUser<Guid>>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString, action => action.MigrationsAssembly(assembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString, action => action.MigrationsAssembly(assembly));
            })
            .AddDeveloperSigningCredential();
        
        
        await SeedData.EnsureSeedDataAsync(connectionString);
    }
}