using Catalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Tests;

public class CatalogApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.Sources.Clear();
            var testSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.Test.json");
            config.AddJsonFile(testSettingsPath, optional: false);
            config.AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            db.Database.EnsureDeleted();
            db.Database.Migrate();
        });
    }
}
