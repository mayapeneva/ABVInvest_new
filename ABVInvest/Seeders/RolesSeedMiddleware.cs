using ABVInvest.Common.Constants;
using ABVInvest.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace ABVInvest.Seeders
{
    public class RolesSeedMiddleware(RequestDelegate next)
    {
        private const string AdminUserName = "ADMIN";
        private const string AdminEmail = "admin@gmail.com";
        private const string AdminPass = "789-Admin";
        private const string PIN = "00000";

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await RolesSeed(userManager, roleManager);
            }

            await next(context);
        }

        private static async Task RolesSeed(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole { Name = ShortConstants.Role.Admin });
            await roleManager.CreateAsync(new IdentityRole { Name = ShortConstants.Role.User });

            var user = new ApplicationUser
            {
                UserName = AdminUserName,
                Email = AdminEmail,
                PIN = PIN,
                FullName = AdminUserName,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var result = await userManager.CreateAsync(user, AdminPass);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, AdminUserName);
            }
        }
    }
}
