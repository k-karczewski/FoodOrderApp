using FoodOrderApp.Interfaces.Providers;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.UserModels;
using FoodOrderApp.Providers;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace FoodOrderApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(SignInManager<UserModel> signInManager, RoleManager<IdentityRole<int>> roleManager, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Signs in user
        /// </summary>
        /// <param name="userToLogin">Username and password of user</param>
        /// <returns>Generated token or errors that occured during sign in process</returns>
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
                    IList<string> usersRoles = await _signInManager.UserManager.GetRolesAsync(user);

                    using(IJsonWebTokenProvider jwtProvider = new JsonWebTokenProvider(_configuration))
                    {
                        string token = jwtProvider.GenerateJwtBearer(user, usersRoles);

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
        /// Registers new user to the application
        /// </summary>
        /// <param name="userToRegister">users data (email, username)</param>
        /// <param name="password">password of the user</param>
        /// <param name="Url">IUrlHelper used to generate account confirmation url</param>
        /// <returns>New user object or list of errors</returns>
        public async Task<IServiceResult<UserModel>> RegisterAsync(UserModel userToRegister, string password, IUrlHelper url)
        {
            try
            {
                IdentityResult result = await _signInManager.UserManager.CreateAsync(userToRegister, password);

                if(result.Succeeded)
                {
                    // assign roles
                    await userToRegister.AssignRole(userToRegister, _signInManager.UserManager, _roleManager);

                    // generate email confirmation token
                    string confirmationToken = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(userToRegister);

                    // convert the token from UTF8 to bytes and then to URL
                    confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

                    string urlLink = url.Action("ConfirmEmail", "Auth", new { userId = userToRegister.Id, token = confirmationToken }, "https");
                    urlLink = HtmlEncoder.Default.Encode(urlLink);

                    using (IEmailSender emailSender = new EmailSender(_configuration))
                    {
                        await emailSender.SendAccountConfirmation(userToRegister, urlLink);
                    }
                                 
                    return new ServiceResult<UserModel>(ResultType.Created, userToRegister);
                }

                List<string> errors = new List<string>();

                foreach (IdentityError identityError in result.Errors)
                {
                    errors.Add(identityError.Description);
                }

                return new ServiceResult<UserModel>(ResultType.Error, errors);

            }
            catch(Exception e)
            {
                UserModel registeredUser = (await _unitOfWork.Users.GetByExpressionAsync(x => x.UserName.ToLower() == userToRegister.UserName.ToLower())).SingleOrDefault();

                if (registeredUser != null)
                {
                    await _signInManager.UserManager.DeleteAsync(registeredUser);
                }

                return new ServiceResult<UserModel>(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Confirms user's email address when generated url was clicked
        /// </summary>
        /// <param name="userId">identifier of user</param>
        /// <param name="confirmationToken">confirmation token sent with confirmation url</param>
        /// <returns>Result status of Correct or Error</returns>
        public async Task<IServiceResult> ConfirmEmailAsync(int userId, string confirmationToken)
        {
            try
            {
                UserModel userToBeConfirmed = (await _unitOfWork.Users.GetByExpressionAsync(x => x.Id == userId)).SingleOrDefault();

                if(userToBeConfirmed != null)
                {
                    string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmationToken));

                    IdentityResult confirmationResult = await _signInManager.UserManager.ConfirmEmailAsync(userToBeConfirmed, decodedToken);

                    if(confirmationResult.Succeeded)
                    {
                        return new ServiceResult(ResultType.Correct);
                    }
                }

                throw new Exception("Error during email confirmation");
            }
            catch(Exception e)
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }
    }
}
