using DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace API.Helpers
{
    public class IdentityDataSeed
    {
        public static async Task SeedData(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            // Seed roles
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new Role() { Name= "Administrator" , ConcurrencyStamp= "Administrator".ToUpper()});
                await roleManager.CreateAsync(new Role() { Name = "User", ConcurrencyStamp = "User".ToUpper() });
            }

            // Seed default user
            var defaultUser = new User { UserName = "admin@example.com", Email = "admin@example.com" };
            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                await userManager.CreateAsync(defaultUser, "AdminPassword123!");
                await userManager.AddToRoleAsync(defaultUser, "Administrator");
            }
        }
    }
}
