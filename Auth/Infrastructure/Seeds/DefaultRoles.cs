using Auth.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Seeds
{
    public static class DefaultRoles
    {
        public static List<IdentityRole> IdentityRoleList()
        {
            List<IdentityRole> identityRoles = new List<IdentityRole>();
            foreach (Roles role in (Roles[])Roles.GetValues(typeof(Roles)))
            {
                identityRoles.Add(new IdentityRole(role.ToString()));
            }

            return identityRoles;
        }
    }
}
