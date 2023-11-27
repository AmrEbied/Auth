namespace Auth.Infrastructure.Models.MultiTenancy
{
    public class Tenant
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? ConnectionString { get; set; }
    }
}
