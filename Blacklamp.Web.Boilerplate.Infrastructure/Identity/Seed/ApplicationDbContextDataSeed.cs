using System;
using System.Threading.Tasks;
using Blacklamp.Web.Boilerplate.Core.Constants;
using Blacklamp.Web.Boilerplate.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Blacklamp.Web.Boilerplate.Infrastructure.Identity.Seed
{
    public class ApplicationDbContextDataSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(ApplicationIdentityConstants.Roles.Administrator));
            await roleManager.CreateAsync(new IdentityRole(ApplicationIdentityConstants.Roles.Member));

            var adminUserName = "admin@mail.com";
            var adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminUserName,
                IsEnabled = true,
                EmailConfirmed = true,
                FirstName = "Vusi",
                LastName = "Magaiana"
            };

            await userManager.CreateAsync(adminUser, ApplicationIdentityConstants.DefaultPassword);
            adminUser = await userManager.FindByEmailAsync(adminUserName);

            await userManager.AddToRoleAsync(adminUser, ApplicationIdentityConstants.Roles.Administrator);
        }
    }
}