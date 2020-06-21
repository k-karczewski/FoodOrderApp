using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodOrderAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Route("new")]
        public async Task<IActionResult> MakeOrder(ICollection<PizzaToOrderDto> orderItems)
        {
            IServiceResult orderResult = await _orderService.MakeOrder(orderItems, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));

            if(orderResult.Result == ResultType.Correct)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            IServiceResult orderResult = await _orderService.CancelOrder(orderId, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));

            if (orderResult.Result == ResultType.Edited)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("delete/{orderId}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            IServiceResult orderResult = await _orderService.DeleteOrder(orderId);

            if (orderResult.Result == ResultType.Deleted)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}