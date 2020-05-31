using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Models.UserModels
{
    public class AdminModel : UserModel
    {
        public override async Task AssignRole(UserModel user, UserManager<UserModel> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            bool roleExists = await roleManager.RoleExistsAsync("Admin");

            if (roleExists == false)
            {
                await roleManager.CreateAsync(new IdentityRole<int>
                {
                    Name = "Admin"
                });
            }

            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
