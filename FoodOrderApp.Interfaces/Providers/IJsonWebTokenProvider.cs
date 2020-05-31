using FoodOrderApp.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Interfaces.Providers
{
    public interface IJsonWebTokenProvider : IDisposable
    {
        /// <summary>
        /// Generates Json Web Token for authorized user
        /// </summary>
        /// <param name="authenticatedUser">User object that just logged in</param>
        /// <param name="roles">roles of the user taken from db</param>
        /// <returns>Generated JWT</returns>
        string GenerateJwtBearer(UserModel authenticatedUser, IList<string> roles);
    }
}
