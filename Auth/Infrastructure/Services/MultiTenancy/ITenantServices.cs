using Auth.Infrastructure.Models.MultiTenancy;

namespace Auth.Infrastructure.Services.MultiTenancy
{
    public interface ITenantServices
    {
        public string? GetDBProvider();
        public string? GetConnectionString();
        public Tenant? GetCurrentTenant();
    }
}
