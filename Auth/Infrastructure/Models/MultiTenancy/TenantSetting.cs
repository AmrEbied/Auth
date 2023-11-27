namespace Auth.Infrastructure.Models.MultiTenancy
{
    public class TenantSetting
    {
        public Configrations Defaults { get; set; } = default!;
        public List<Tenant> Tenant { get; set; } = new();
    }
}
