using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Models.UserModels
{
    public class UserModel : IdentityUser<int>
    {
        public virtual async Task AssignRole(UserModel user, UserManager<UserModel> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            bool roleExists = await roleManager.RoleExistsAsync("User");

            if (roleExists == false)
            {
                await roleManager.CreateAsync(new IdentityRole<int>
                {
                    Name = "User"
                });
            }

            await userManager.AddToRoleAsync(user, "User");
        }
    }
}
