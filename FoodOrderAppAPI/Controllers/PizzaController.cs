using System.Collections.Generic;
using System.Threading.Tasks;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodOrderAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class PizzaController : ControllerBase
    {
        private readonly IPizzaService _service;

        public PizzaController(IPizzaService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Create(PizzaToCreateDto pizzaToCreate)
        {
            IServiceResult<PizzaToReturnDto> result = await _service.CreateAsync(pizzaToCreate);

            if (result.Result == ResultType.Created)
            {
                return CreatedAtRoute("GetPizzaByName", new { name = result.ReturnedObject.Name }, result.ReturnedObject);
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet("name/{name}", Name = "GetPizzaByName")]
        public async Task<IActionResult> GetByName(string name)
        {
            IServiceResult<PizzaToReturnDto> result = await _service.GetByNameAsync(name);

            if (result.Result == ResultType.Correct)
            {
                return Ok(result.ReturnedObject);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet("{id}", Name = "GetPizzaById")]
        public async Task<IActionResult> GetById(int id)
        {
            IServiceResult<PizzaModel> result = await _service.GetByIdAsync(id);

            if (result.Result == ResultType.Correct)
            {
                return Ok(result.ReturnedObject);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            IServiceResult<List<PizzaToReturnDto>> result = await _service.GetAsync();

            if (result.Result == ResultType.Correct)
            {
                return Ok(result.ReturnedObject);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{pizzaName}/add-ingredient/{ingredientId}")]
        public async Task<IActionResult> AddIngredient(string pizzaName, int ingredientId)
        {
            IServiceResult<PizzaToReturnDto> result = await _service.AddIngredientAsync(pizzaName, ingredientId);

            if (result.Result == ResultType.Edited)
            {
                return CreatedAtRoute("GetPizzaByName", new { name = result.ReturnedObject.Name }, result.ReturnedObject);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{pizzaName}/delete-ingredient/{ingredientId}")]
        public async Task<IActionResult> DeleteIngredient(string pizzaName, int ingredientId)
        {
            IServiceResult<PizzaToReturnDto> result = await _service.DeleteIngredientAsync(pizzaName, ingredientId);

            if (result.Result == ResultType.Edited)
            {
                return CreatedAtRoute("GetPizzaByName", new { name = result.ReturnedObject.Name }, result.ReturnedObject);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}