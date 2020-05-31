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
        /// <summary>
        /// Signs in user
        /// </summary>
        /// <param name="userToLogin">Username and password of user</param>
        /// <returns>Generated token or errors that occured during sign in process</returns>
        Task<IServiceResult<string>> LoginAsync(UserToLoginDto userToLogin);
        
        /// <summary>
        /// Registers new user to the application
        /// </summary>
        /// <param name="userToRegister">users data (email, username)</param>
        /// <param name="password">password of the user</param>
        /// <param name="Url">IUrlHelper used to generate account confirmation url</param>
        /// <returns>New user object or list of errors</returns>
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
