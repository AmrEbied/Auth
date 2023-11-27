using Auth.Domain.Enums;
using Auth.Infrastructure.Services.MultiTenancy;
using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Entities
{
    public class ApplicationUser : IdentityUser, IMustHaveTenant
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!; 
        public TypeUser TypeUser { get; set; }
        public bool ActiveCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int Code { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; } 
        public List<RefreshToken>? RefreshTokens { get; set; }
        public List<UserDevice>? UserDevices { get; set; }
        public string TenantId { get; set; }=null!;
    }
}
