using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Microsoft.AspNetCore.Identity;
namespace Auth.Infrastructure.Seeds
{
    public static class ContextSeed
    {
        
        public static async Task Seed(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,string tenantId)
        { 
            await CreateRoles(roleManager);
            await CreateBasicUsers(userManager, tenantId); 
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (IdentityRole role in DefaultRoles.IdentityRoleList())
            {
                await roleManager.CreateAsync(role);
            }
        }

        private static async Task CreateBasicUsers(UserManager<ApplicationUser> userManager,string tenantId)
        {
            foreach (ApplicationUser user in DefaultUser.IdentityBasicUserList(tenantId))
            {
                var userFound = await userManager.FindByEmailAsync(user.Email);
                if (userFound == null)
                { 
                    await userManager.CreateAsync(user, "P@ssw0rd");

                    if (user.TypeUser ==TypeUser.Admin)
                    {
                        await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(user, Roles.User.ToString());
                    }
                }
            }
        }

    }
}
