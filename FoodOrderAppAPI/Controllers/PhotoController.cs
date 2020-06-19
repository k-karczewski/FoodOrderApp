using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderAppAPI.Controllers
{
    [ApiController]
    [Authorize(Policy = "RequireAdminRole")]
    [Route("api/[controller]/")]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;

        public PhotoController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddPizzaPhotoAsync(PhotoToCreateDto photoToCreate)
        {
            IServiceResult result = await _photoService.AddPizzaPhotoAsync(photoToCreate);

            if (result.Result == ResultType.Correct)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}