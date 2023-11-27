using Auth.Domain.Entities;
using Auth.Domain.Enums;

namespace Auth.Infrastructure.Seeds
{
    public static class DefaultUser
    {
        public static List<ApplicationUser> IdentityBasicUserList(string tenantId)
        {
            return new List<ApplicationUser>()
            {
                new ApplicationUser
                {
                    UserName = "AmrEbied",
                    Email = "amr@gmail.com",
                    TypeUser = TypeUser.User,
                    ActiveCode = true,
                    CreatedAt = DateTime.Now,
                    IsActive = true, 
                    Code=1234,
                    IsDeleted=false,
                    FirstName="Amr",
                    LastName="Ebied",
                    TenantId=tenantId
                },

            };
        }
    }
}
