using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.UserModels;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Services
{
    public class SignUpService : ISignUpService
    {
        private readonly UserManager<UserModel> _userManager;

        public SignUpService(UserManager<UserModel> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Creates/Registers new user in database
        /// </summary>
        /// <param name="userToRegister">Correct user data from registration form - model state has been checked in controller layer</param>
        /// <returns>Service result statuses: Created or Error</returns>
        public async Task<IServiceResult<UserModel>> RegisterAsync(UserToRegisterDto userToRegister)
        {
            UserModel user = new UserModel
            {
                UserName = userToRegister.Username,
                Email = userToRegister.EmailAddress
            };

            IdentityResult result = await _userManager.CreateAsync(user, userToRegister.Password);

            if(result.Succeeded)
            {
                return new ServiceResult<UserModel>(ResultType.Created, user);
            }

            List<string> errors = new List<string>();

            foreach(IdentityError identityError in result.Errors)
            {
                errors.Add(identityError.Description);
            }

            return new ServiceResult<UserModel>(ResultType.Error, errors);
        }
    }
}
