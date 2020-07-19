using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Interfaces.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Creates new order
        /// </summary>
        /// <param name="pizzas">List of pizzas to order</param>
        /// <param name="userId">id of user that makes order</param>
        /// <returns>ServiceResult of statuses correct or error</returns>
        Task<IServiceResult> MakeOrder(List<PizzaToOrderDto> pizzas, int userId);

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="orderId">id of order to be canceled</param>
        /// <param name="userId">id of user that order belongs to</param>
        /// <returns>ServiceResult of statuses edited or error</returns>
        Task<IServiceResult> CancelOrder(int orderId, int userId);

        /// <summary>
        /// Deletes order from database
        /// </summary>
        /// <param name="orderId">id of order to be deleted</param>
        /// <returns>ServiceResult of statuses deleted or error</returns>
        Task<IServiceResult> DeleteOrder(int orderId);
    }
}
