using BestStore.Models;
using Microsoft.AspNetCore.Identity;

namespace BestStore.Services
{
    public class DataBaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser>? userManager,
            RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null && roleManager == null)
            {
                Console.WriteLine("userManager or rolemanager is null => exit");
                return;
            }

            //check if we have admin role or not

            var exists = await roleManager.RoleExistsAsync("admin");
            if (!exists)
            {
                Console.WriteLine("admin role is not defined and will created");
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }


            //check if we have seller role or not

            exists = await roleManager.RoleExistsAsync("seller");
            if (!exists)
            {
                Console.WriteLine("seller role is not defined and will created");
                await roleManager.CreateAsync(new IdentityRole("seller"));
            }

            //check if we have cleint role or not

            exists = await roleManager.RoleExistsAsync("cleint");
            if (!exists)
            {
                Console.WriteLine("cleint role is not defined and will created");
                await roleManager.CreateAsync(new IdentityRole("cleint"));
            }


            var adminUsers = await userManager.GetUsersInRoleAsync("admin");
            if(adminUsers.Any())
            {
                Console.WriteLine("Admin users already exists => exists");
                return;
            }


            var user = new ApplicationUser()
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                CreateAt = DateTime.Now,
            };
            string initialPassword = "admin123";


            var result= await userManager.CreateAsync(user, initialPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "admin");
                Console.WriteLine("admin user created");
                Console.WriteLine("Email "+user.Email);
                Console.WriteLine("InitialPassword :"+initialPassword);
            }
        }
    }
}
