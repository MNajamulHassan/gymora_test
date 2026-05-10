using Gymora.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Gymora.Data
{
    public static class SeedData
    {
        private static readonly Guid DemoTenantId = new Guid("11111111-1111-1111-1111-111111111111");

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Create roles
            string[] roles = { "SuperAdmin", "GymOwner", "Receptionist", "Trainer", "Member" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed GymOwner
            var gymOwnerEmail = "admin@ironpeak.com";
            if (await userManager.FindByEmailAsync(gymOwnerEmail) == null)
            {
                var gymOwner = new ApplicationUser
                {
                    UserName = gymOwnerEmail,
                    Email = gymOwnerEmail,
                    FullName = "Gym Owner",
                    TenantId = DemoTenantId,
                    IsActive = true,
                    JoinDate = DateTime.Today,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await userManager.CreateAsync(gymOwner, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(gymOwner, "GymOwner");
                }
            }

            // 3. Seed Receptionist
            var receptionEmail = "reception@ironpeak.com";
            if (await userManager.FindByEmailAsync(receptionEmail) == null)
            {
                var receptionist = new ApplicationUser
                {
                    UserName = receptionEmail,
                    Email = receptionEmail,
                    FullName = "Front Desk",
                    TenantId = DemoTenantId,
                    IsActive = true,
                    JoinDate = DateTime.Today,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await userManager.CreateAsync(receptionist, "Recep@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(receptionist, "Receptionist");
                }
            }
        }
    }
}
