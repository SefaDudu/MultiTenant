using Core.Application.Abstraction.Tenant;
using Core.Application.Settings;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureService(this IServiceCollection collection)
        {
            collection.AddTransient<ITenantService, TenantService>();
        }
        public static async Task AddPersistenceService(this IServiceCollection collection)
        {
            using var provider = collection.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var tenantSettings = configuration.GetSection("TenantSettings").Get<TenantSettings>();

            var defaultConnectionString = tenantSettings.Defaults?.ConnectionString;
            var defaultDbProvider = tenantSettings.Defaults?.DbProvider;
            if (defaultDbProvider.ToLower() == "mssql")
                collection.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(e => e.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            using IServiceScope scope = collection.BuildServiceProvider().CreateScope();

            foreach (var tenant in tenantSettings.Tenants)
            {
                string connectionString = tenant.ConnectionString switch
                {
                    null => defaultConnectionString,
                    not null => tenant.ConnectionString
                };
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.SetConnectionString(connectionString);
                if (dbContext.Database.GetMigrations().Count() > 0)
                    await dbContext.Database.MigrateAsync();
            }
        }
    }
}
