using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Services
{
    public interface ISignUpService
    {
        Task<IServiceResult<UserModel>> RegisterAsync(UserToRegisterDto userToCreate);
    }
}
