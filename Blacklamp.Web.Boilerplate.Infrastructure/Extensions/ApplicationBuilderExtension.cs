using System.Security.Policy;
using System.Threading.Tasks;
using Blacklamp.Web.Boilerplate.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Blacklamp.Web.Boilerplate.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static void EnsureIdentityDbIsCreated(this IApplicationBuilder builder)
        {
            using var serviceScope =
                builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var services = serviceScope.ServiceProvider;
            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.EnsureCreated();
        }

        public static async Task SeedIdentityDataAsync(this IApplicationBuilder builder)
        {
            using var serviceScope =
                builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var services = serviceScope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetService<RoleManager<IdentityRole>>();

            await Identity.Seed.ApplicationDbContextDataSeed.SeedAsync(userManager, roleManager);
        }
    }
}