//Seeder --> Data Seedin(Veri Tohumlama) admin user role muhabbeti

using Microsoft.AspNetCore.Identity;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Services
{
    public class UserSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            //Servisler --> Kullanıcı ve rol yoneticileri
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //Rolleri olusturma (Admin ve uye)
            string[] roleNames = { "Admin", "Uye" };

            foreach (var roleName in roleNames)
            {
                //Eger rol DB'de yoksa olustur
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Admin Kullanicisini olusturma
            string adminEmail = "B241210380@sakarya.edu.tr";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if(adminUser == null)
            {
                //admin yoksa olustur
                var newAdmin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Sistem",
                    LastName = "Yöneticisi",
                    EmailConfirmed = true, // eposta onayli olmasi icin
                };

                var result = await userManager.CreateAsync(newAdmin, "sau");

                if(result.Succeeded)
                {
                    //olusturaln kullaniciya admin rolu verme
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}
