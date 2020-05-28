using FoodOrderApp.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Interfaces.Providers
{
    public interface IJsonWebTokenProvider : IDisposable
    {
        string GenerateJwtBearer(UserModel authenticatedUser);
    }
}
