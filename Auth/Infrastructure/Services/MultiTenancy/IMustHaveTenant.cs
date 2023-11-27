namespace Auth.Infrastructure.Services.MultiTenancy
{
    public interface IMustHaveTenant
    {
        public string TenantId { get; set; }
    }
}
