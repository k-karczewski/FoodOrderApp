using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _service;

        public IngredientController(IIngredientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetIngredients()
        {
            IServiceResult<List<IngredientModel>> result = await _service.GetAsync();

            if (result.Result == ResultType.Correct)
            {
                return Ok(result.ReturnedObject);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpGet("{id}", Name="GetIngedientById")]
        public async Task<IActionResult> GetIngredientById(int id)
        {
            IServiceResult<IngredientModel> result = await _service.GetByIdAsync(id);

            if(result.Result == ResultType.Correct)
            {
                return Ok(result.ReturnedObject);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateIngredient(IngredientModel ingredient)
        {
            IServiceResult<IngredientModel> result = await _service.CreateAsync(ingredient);

            if(result.Result == ResultType.Created)
            {
                return CreatedAtRoute("GetIngedientById", new { id = result.ReturnedObject.Id }, result.ReturnedObject);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPut]
        [Route("change-price/{ingredientId}")]
        public async Task<IActionResult> ChangePrice(IngredientPriceModel price, int ingredientId)
        {
            IServiceResult<IngredientModel> result = await _service.UpdatePriceAsync(price, ingredientId);

            if(result.Result == ResultType.Edited)
            {
                return CreatedAtRoute("GetIngedientById", new { id = result.ReturnedObject.Id }, result.ReturnedObject);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            IServiceResult result = await _service.DeleteAsync(id);

            if(result.Result == ResultType.Deleted)
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