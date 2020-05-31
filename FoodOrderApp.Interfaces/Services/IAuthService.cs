using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.UserModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Services
{
    public interface IAuthService
    {
        Task<IServiceResult<string>> LoginAsync(UserToLoginDto userToLogin);
        Task<IServiceResult<UserModel>> RegisterAsync(UserModel userToRegister, string password, IUrlHelper Url);
        /// <summary>
        /// Confirms user's email address when generated url was clicked
        /// </summary>
        /// <param name="userId">identifier of user</param>
        /// <param name="confirmationToken">confirmation token sent with confirmation url</param>
        /// <returns>Result status of Correct or Error</returns>
        Task<IServiceResult> ConfirmEmailAsync(int userId, string confirmationToken);
    }
}
