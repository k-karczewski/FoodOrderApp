using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
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

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(PizzaToCreateDto pizzaToCreate)
        {
            IServiceResult<List<PizzaModel>> result = await _service.CreateAsync(pizzaToCreate);

            if(result.Result == ResultType.Created)
            {
               return CreatedAtRoute("GetByName", new { name = result.ReturnedObject[0].Name }, result.ReturnedObject);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("GetByName")]
        [Route("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            IServiceResult<List<PizzaToReturnDto>> result = await _service.GetByName(name);

            if(result.Result == ResultType.Correct)
            {
                return Ok(result.ReturnedObject);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


        [HttpGet("GetById")]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            IServiceResult<PizzaModel> result = await _service.GetByIdAsync(id);

            if (result.Result == ResultType.Created)
            {
                return Ok(result.ReturnedObject);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
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
    }
}