using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderAppAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService signUpService)
        {
            _authService = signUpService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            IServiceResult<ICollection<string>> result = await _authService.GetUsersNames();

            return Ok(result.Errors);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(UserToRegisterDto userToRegister)
        {
            if(ModelState.IsValid)
            {
                IServiceResult<UserModel> registerResult = await _authService.RegisterAsync(userToRegister);

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
                    return Ok(loginResult.ReturnedObject);
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

    }
}