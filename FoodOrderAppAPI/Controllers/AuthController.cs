using System.Threading.Tasks;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.Enums;
using FoodOrderApp.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderAppAPI.Controllers
{
    [Authorize]
    [ApiController] 
    [Route("api/[controller]/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService signUpService)
        {
            _authService = signUpService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(UserToRegisterDto userToRegister)
        {
            if(ModelState.IsValid)
            {
                UserModel user = new UserModel
                {
                    UserName = userToRegister.Username,
                    Email = userToRegister.EmailAddress
                };

                IServiceResult<UserModel> registerResult = await _authService.RegisterAsync(user, userToRegister.Password, Url);

                if(registerResult.Result == ResultType.Created)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(registerResult.Errors);
                }
            }
            else
            {
                return BadRequest(ModelState.Values);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserToLoginDto userToLogin)
        {
            if (ModelState.IsValid)
            {
                IServiceResult<string> loginResult = await _authService.LoginAsync(userToLogin);

                if (loginResult.Result == ResultType.Correct)
                {
                    return Ok(new
                    {
                        token = loginResult.ReturnedObject
                    });
                }
                else
                {
                    return BadRequest(loginResult.Errors);
                }
            }
            else
            {
                return BadRequest(ModelState.Values);
            }
        }

        [AllowAnonymous]
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            IServiceResult confirmationResult = await _authService.ConfirmEmailAsync(userId, token);

            if (confirmationResult.Result == ResultType.Correct)
            {
                return Ok();
            }

            return BadRequest(confirmationResult.Errors);
        }

        [HttpPost("admin/register")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> RegisterAdminAsync(UserToRegisterDto userToRegister)
        {
            if (ModelState.IsValid)
            {
                UserModel user = new AdminModel
                {
                    UserName = userToRegister.Username,
                    Email = userToRegister.EmailAddress
                };

                IServiceResult<UserModel> registerResult = await _authService.RegisterAsync(user, userToRegister.Password, Url);

                if (registerResult.Result == ResultType.Created)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(registerResult.Errors);
                }
            }
            else
            {
                return BadRequest(ModelState.Values);
            }
        }
    }
}