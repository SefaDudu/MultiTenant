using Core.Application.Abstraction.Tenant;
using Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence
{
    public class ApplicationDbContext:DbContext
    {
        readonly ITenantService _tenantService;
        string tenantId;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService) : base(options)
        {
            _tenantService = tenantService;
            tenantId = _tenantService.GetTenant()?.TenantId;
        }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == tenantId);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var tenantConnectionString = _tenantService.GetConnectionString();
            if (!string.IsNullOrEmpty(tenantConnectionString))
            {
                var dbProvider = _tenantService.GetDatabaseProvider();
                if (dbProvider.ToLower() == "postgres")
                    optionsBuilder.UseSqlServer(tenantConnectionString);
            }
        }
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        entry.Entity.TenantId = tenantId;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        }
}
