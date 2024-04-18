using Microsoft.AspNetCore.Identity;
using StripeItegration.DbContext;
using StripeItegration.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace StripeItegration.Config
{
    public static class RoleConfig
    {
        public static async void SetupRole(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {  
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (!db.Database.CanConnect()) return;

                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var role =  "Admin";
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                if (app.Environment.IsDevelopment())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    if (userManager.FindByNameAsync("Admin").Result == null)
                    {
                        var adminUser = new ApplicationUser();
                        adminUser.UserName = "Admin";
                        adminUser.Email = "Admin@ADmin.com";
                        await userManager.CreateAsync(adminUser, "Admin#1");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
    }
}
