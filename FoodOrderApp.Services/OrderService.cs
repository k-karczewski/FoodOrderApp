using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.Enums;
using FoodOrderApp.Models.OrderModels;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.DetailModels;
using FoodOrderApp.Models.UserModels;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _repository;

        public OrderService(IUnitOfWork repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Creates new order
        /// </summary>
        /// <param name="pizzasToOrder">List of pizzas to order</param>
        /// <param name="userId">id of user that makes order</param>
        /// <returns>ServiceResult of statuses correct or error</returns>
        public async Task<IServiceResult> MakeOrder(List<PizzaToOrderDto> pizzasToOrder, int userId)
        {
            try
            {
                UserModel purchaser = (await _repository.Users.GetByExpressionAsync(x => x.Id == userId)).SingleOrDefault();

                if(purchaser != null)
                {
                    List<OrderItemModel> orderItems = await ConvertToOrderItems(pizzasToOrder);

                    if(orderItems != null)
                    {
                        OrderModel order = new OrderModel
                        {
                            UserId = purchaser.Id,
                            User = purchaser,
                            OrderItems = orderItems,
                            Status = OrderStatus.New
                        };

                        await _repository.Orders.CreateAsync(order);
                        await _repository.SaveChangesAsync();

                        return new ServiceResult(ResultType.Correct);
                    }
                }

                return new ServiceResult(ResultType.Error, new List<string> { "Error during creation of order" });
            }
            catch(Exception e) 
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="orderId">id of order to be canceled</param>
        /// <param name="userId">id of user that order belongs to</param>
        /// <returns>ServiceResult of statuses edited or error</returns>
        public async Task<IServiceResult> CancelOrder(int orderId, int userId)
        {
            try
            {
                OrderModel orderToCancel = (await _repository.Orders.GetByExpressionAsync(x => x.Id == orderId)).SingleOrDefault();

                if(orderToCancel != null && orderToCancel.UserId == userId)
                {
                    orderToCancel.Status = OrderStatus.Canceled;
                    _repository.Orders.Update(orderToCancel);
                    await _repository.SaveChangesAsync();

                    return new ServiceResult(ResultType.Edited);
                }

                return new ServiceResult(ResultType.Error, new List<string> { "Error during order cancel operation" });
            }
            catch(Exception e)
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

        /// <summary>
        /// Deletes order from database
        /// </summary>
        /// <param name="orderId">id of order to be deleted</param>
        /// <returns>ServiceResult of statuses deleted or error</returns>
        public async Task<IServiceResult> DeleteOrder(int orderId)
        {
            try
            {
                OrderModel orderToDelete = (await _repository.Orders.GetByExpressionAsync(x => x.Id == orderId, i => i.Include(po => po.OrderItems))).SingleOrDefault();

                // order can be deleted only by user with admin privileges so check of userId is not needed
                if (orderToDelete != null)
                {
                    _repository.Orders.Delete(orderToDelete);
                    await _repository.SaveChangesAsync();

                    return new ServiceResult(ResultType.Deleted);
                }

                return new ServiceResult(ResultType.Error, new List<string> { $"Order with id {orderId} has not been found" });
            }
            catch(Exception e)
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }

        #region PrivateMethods
        private async Task<List<OrderItemModel>> ConvertToOrderItems(List<PizzaToOrderDto> orderItemDtos)
        {
            List<OrderItemModel> orderItems = new List<OrderItemModel>();

            try
            {
                foreach (PizzaToOrderDto item in orderItemDtos)
                {
                    // get pizza with id that came from spa
                    PizzaModel pizzaToOrder = (await _repository.Pizzas.GetByExpressionAsync(x => x.Id == item.Id)).SingleOrDefault();

                    OrderItemModel orderItem = new OrderItemModel
                    {
                        Name = pizzaToOrder.Name,
                        Size = item.Size,
                        Quantity = item.Quantity,
                        OrderItemIngredients = new List<OrderIngredientModel>()
                    };

                    foreach (int id in item.Ingredients)
                    {
                        orderItem.OrderItemIngredients.Add(new OrderIngredientModel
                        {
                            IngredientId = id
                        });
                    }

                    orderItem.Price = await CalculateOrderItemPrice(orderItem.OrderItemIngredients.Select(x => x.IngredientId).ToList(), orderItem.Size, item.Quantity);

                    // validate price of order item
                    if(orderItem.Price != item.Price)
                    {
                        return null;
                    }

                    orderItems.Add(orderItem);
                }
            
                return orderItems;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private async Task<decimal> CalculateOrderItemPrice(List<int> orderItemIngredientsIds, SizeEnum size, int quantity)
        {
            try
            {
                decimal total = (await _repository.Starters.GetByExpressionAsync(x => x.Size == size)).Single().Price;

                foreach(int id in orderItemIngredientsIds)
                {
                    IngredientModel i = (await _repository.Ingredients.GetByExpressionAsync(x => x.Id == id, i => i.Include(d => d.IngredientDetails))).Single();
                    total += i.IngredientDetails.FirstOrDefault(x => x.Size == size).Price;
                }

                return total * quantity;
            }
            catch(Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
