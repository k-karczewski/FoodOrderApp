using FoodOrderApp.Interfaces.Providers;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.UserModels;
using FoodOrderApp.Providers;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }


        public async Task<IServiceResult<ICollection<string>>> GetUsersNames()
        {
            ICollection<string> usernames = new List<string>();

            List<UserModel> users = (await _unitOfWork.Users.GetByExpressionAsync(x => x.Id > 0)).ToList();

            foreach(UserModel user in users)
            {
                usernames.Add(user.UserName);
            }

            return new ServiceResult<ICollection<string>>(ResultType.Correct, usernames);
        }

        public async Task<IServiceResult<string>> LoginAsync(UserToLoginDto userToLogin)
        {
            try
            {       
                UserModel user = (await _unitOfWork.Users.GetByExpressionAsync(x => x.UserName.ToLower() == userToLogin.Username.ToLower())).SingleOrDefault();

                if(user == null)
                {
                    throw new Exception("Username and password do not match");
                }

                SignInResult loginResult = await _signInManager.CheckPasswordSignInAsync(user, userToLogin.Password, false);

                if(loginResult.Succeeded)
                {
                    using(IJsonWebTokenProvider jwtProvider = new JsonWebTokenProvider(_configuration))
                    {
                        string token = jwtProvider.GenerateJwtBearer(user);

                        return new ServiceResult<string>(ResultType.Correct, token);
                    }
                }
                else
                {
                    throw new Exception("Username and password do not match");
                }
            }
            catch(Exception e)
            {
                return new ServiceResult<string>(ResultType.Unauthorized, new List<string> { e.Message });
            }
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
