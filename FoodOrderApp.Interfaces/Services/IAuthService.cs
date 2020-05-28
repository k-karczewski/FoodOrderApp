using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Services
{
    public interface IAuthService
    {
        Task<IServiceResult<ICollection<string>>> GetUsersNames();
        Task<IServiceResult<string>> LoginAsync(UserToLoginDto userToLogin);
        Task<IServiceResult<UserModel>> RegisterAsync(UserToRegisterDto userToCreate);
    }
}
