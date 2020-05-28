using System;
using System.Threading.Tasks;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.UserModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class AuthController : ControllerBase
    {
        private readonly ISignUpService _signUpService;

        public AuthController(ISignUpService signUpService)
        {
            _signUpService = signUpService;
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(UserToRegisterDto userToRegister)
        {
            if(ModelState.IsValid)
            {
                IServiceResult<UserModel> registerResult = await _signUpService.RegisterAsync(userToRegister);

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

    }
}