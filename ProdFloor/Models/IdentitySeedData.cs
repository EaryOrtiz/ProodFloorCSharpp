using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ProdFloor.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Secret123$";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            UserManager<AppUser> userManager = app.ApplicationServices
            .GetRequiredService<UserManager<AppUser>>();
            AppUser user = await userManager.FindByIdAsync(adminUser);
            if (user == null)
            {
                user = new AppUser { UserName = adminUser};
                await userManager.CreateAsync(user, adminPassword);
            }
            RoleManager<IdentityRole> roleManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();
            IdentityRole role = await roleManager.FindByNameAsync("Admin");
            if(role == null)
            {
                role = new IdentityRole { Name = "Admin" };
                await roleManager.CreateAsync(role);
                await userManager.AddToRoleAsync(user, role.Name);
            }
            
        }
    }
}