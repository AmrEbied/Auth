using Auth.Infrastructure.Services.MultiTenancy;
using AzureBlobManager.Infrastructure.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Auth.Domain.Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public string TenantId { get; set; }
        private readonly ITenantServices _tenantService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantServices tenantService) : base(options)
        {
            _tenantService = tenantService;
            TenantId = _tenantService.GetCurrentTenant()?.Id;
        }

        public virtual DbSet<Document>? Documents { get; set; }
        public virtual DbSet<LookupMediaType>? LookupMediaTypes { get; set; }
        public virtual DbSet<Configuration>? Configurations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // you can filtter here to all tables like this
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(e => e.TenantId == TenantId);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var tenantConnectionString = _tenantService.GetConnectionString();

            if (!string.IsNullOrWhiteSpace(tenantConnectionString))
            {
                var dbProvider = _tenantService.GetDBProvider();

                if (dbProvider?.ToLower() == "mssql")
                {
                    optionsBuilder.UseSqlServer(tenantConnectionString);
                }
                //else if (dbProvider?.ToLower() == "Nosql")
                // {
                //     optionsBuilder.UseSqlServer(tenantConnectionString);
                // }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // selecct all entities that implement from IMustHaveTenant
            // any insert in tables will take TenantId
            foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().Where(e => e.State == EntityState.Added))
            {
                if(TenantId!=null)
                entry.Entity.TenantId = TenantId;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
